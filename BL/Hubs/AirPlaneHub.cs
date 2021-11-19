using DAL;
using Microsoft.AspNetCore.SignalR;
using Models;
using System;
using System.Threading;

namespace BL.Hubs
{

    public class AirPlaneHub : Hub
    {
        private readonly IHubContext<AirPlaneHub> _context;
        public AirPlaneHub(IHubContext<AirPlaneHub> context) => _context = context;

        public async void PlaneRemovedFromAirPort(string planeId) => await Clients.All.SendAsync("PlaneRemovedFromAirPort", planeId);
        public async void Emptystation(string station) => await Clients.All.SendAsync("Emptystation", station);
        public async void PlaneStart(string planeID, bool IsLanding) => await Clients.All.SendAsync("PlaneStart", planeID, IsLanding);
        public async void PlaneFinished(string planeID) => await Clients.All.SendAsync("PlaneFinished", planeID);
        public async void PlaneEnterStation(string planeID, string stationID) => await _context.Clients.All.SendAsync("PlaneEnterStation", planeID, stationID);
        public async void PlaneExitStation(string planeID, string stationID) => await _context.Clients.All.SendAsync("PlaneExitStation", planeID, stationID);
        public async void TryRemovePlane(string planeId) => await _context.Clients.All.SendAsync("TryRemovePlane", planeId);
        public async void ActiveRefresh()
        {
            Exception ex = new Exception();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var data = await FileWorker.LoadJsonAsync<StationStatusModel>(Config.StationStatusPath);
                    await _context.Clients.All.SendAsync("Refresh", data);
                    // failed await Clients.Caller.SendAsync("Refresh", data);
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                    Thread.Sleep(3);
                }
            }
            FileWorker.WriteToLog(new LogError
            {
                ErrorTime = DateTime.Now,
                ExceptionThrown = ex,
                Info = "thrown at ActiveRefresh"
            });

        }
    }
}
