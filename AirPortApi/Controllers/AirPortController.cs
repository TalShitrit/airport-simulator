using BL;
using BL.API;
using BL.Implementation;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System.Collections.Generic;

namespace AirPortApi.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AirPortController : ControllerBase
    {
        private readonly IAirportLogic _airportLogic;
        private readonly IAirport _airport;
        private readonly HubConnection _connection;
        static int count = 0;

        public AirPortController(IAirportLogic airportLogic, IAirport airport)
        {
            _airportLogic = airportLogic;
            this._airport = airport;
            _connection = new HubConnectionBuilder()
                          .WithUrl(Config.API + Config.Hub)
                          .WithAutomaticReconnect()
                          .Build();
            StartConnection();
        }
   
        [HttpGet("home")]
        public string Home() => "hey";
        [HttpGet("landing")]
        public void Landing()
        {
            count++;
            Plane plane = new Plane(new LandingRoute(_airport), _airportLogic, count.ToString());
            _airportLogic.StartMove(plane);
        }
        [HttpGet("TakeOff")]
        public void TakeOff()
        {
            count++;
            Plane plane = new Plane(new TakeOffRoute(_airport), _airportLogic, count.ToString());
            _airportLogic.StartMove(plane);
        }
        [HttpGet("Simulator")]
        public void Simulator()
        {
            Landing();
            Landing();
            Landing();
            TakeOff();
            TakeOff();
            Landing();
        }

        private async void StartConnection()
        {
            await _connection.StartAsync();
            _connection.On<List<StationStatusModel>>("Refresh", (stationStatusModels) =>
            {
                _airportLogic.Refresh();
                _airport.Refresh();
            });
            _connection.On<string>("TryRemovePlane", (planeId) =>
            {
                _airport.TryTakeOutPlane(planeId);
            });
        }
    }
}
