using BL.API;
using DAL;
using DAL.API;
using DAL.Implementation;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;

namespace BL
{
    public static class MyOutPut
    {
        private static readonly IPlaneChange _filePlane;
        private static readonly IStationStatus _stationStatus;
        private static readonly IPlaneQueue _queueFile;
        private static readonly HubConnection _connection;
        static MyOutPut()
        {

            _connection = new HubConnectionBuilder()
                            .WithUrl(Config.API + Config.Hub)
                            .WithAutomaticReconnect()
                            .Build();
            StartConnection();
            _filePlane = new PlaneChange();
            _queueFile = new PlaneQueueFile();
            _stationStatus = new StationStatusFiles();
            _filePlane.ResetFile();
        }

        public static void StartStaticCTor() { }// for testing
        public async static void PlaneStart(IPlane plane)
        {
            if (plane != null)
            {
                await _filePlane.StartAsync(GetPlaneModel(plane));
                await _connection.SendAsync("PlaneStart", plane.GetId, plane.IsLanding);
            }
            else
                FileWorker.WriteToLog("At PlaneStart of MyOutPut plane was null");
        }
        public async static void PlaneEnterStation(IPlane plane, IAirportStation station)
        {
            if (plane != null && station != null)
            {
                await _filePlane.EnterStationAsync(GetPlaneModel(plane, station));
                await _connection.SendAsync("PlaneEnterStation", plane.GetId, station.Id);
            }
            else
                FileWorker.WriteToLog("At PlaneExitStation of MyOutPut plane or station were null");
        }
        public static void PlaneEnterQueuePlan(IPlane plane)
        {
            if (plane != null)
                _queueFile.PlaneEnterQueue(GetPlaneModel(plane));
            else
                FileWorker.WriteToLog("At PlaneFinished of MyOutPut plane was null");
        }
        public static void PlaneExitQueuePlan(IPlane plane)
        {
            if (plane != null)
            {
                _queueFile.PlaneExitQueue(plane.IsLanding);
                PlaneStart(plane);
            }
            else
                FileWorker.WriteToLog("At PlaneExitQueuePlan of MyOutPut plane was null");
        }
        public async static void PlaneExitStation(IPlane plane, IAirportStation station)
        {
            if (plane != null && station != null)
            {
                await _filePlane.ExitStationAsync(GetPlaneModel(plane, station));
                await _connection.SendAsync("PlaneExitStation", plane.GetId, station.Id);
            }
            else
                FileWorker.WriteToLog("At PlaneExitStation of MyOutPut plane or station were null");
        }
        public async static void PlaneFinished(IPlane plane)
        {
            if (plane != null)
            {
                await _filePlane.FinishAsync(GetPlaneModel(plane));
                await _connection.SendAsync("PlaneFinished", plane.GetId);
            }
            else
                FileWorker.WriteToLog("At PlaneFinished of MyOutPut plane was null");
        }
        public async static void PlaneRemovedFromAirPort(string planeId, bool fromPlanTo = false)
        {
            try
            {
                if (fromPlanTo is false)
                    _stationStatus.EmptyStationByPlaneId(planeId);
                await _filePlane.PlaneRemovedFromAirPort(planeId);
                await _connection.SendAsync("PlaneRemovedFromAirPort", planeId);
            }
            catch (Exception e)
            {
                FileWorker.WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = e,
                    Info = $"at function PlaneRemovedFromAirPort with plane id {planeId}"
                });
            }

        }

        private static async void StartConnection() => await _connection.StartAsync();
        private static PlaneModel GetPlaneModel(IPlane plane) => new PlaneModel { PlaneID = plane.GetId, Time = DateTime.Now, IsLanding = plane.IsLanding, StationIndex = plane.StationIndex };
        private static PlaneModel GetPlaneModel(IPlane plane, IAirportStation station) => new PlaneModel
        {
            PlaneID = plane.GetId,
            StationFrom = plane.StationId,
            StationTo = station.Id,
            Time = DateTime.Now,
            IsLanding = plane.IsLanding,
            StationIndex = plane.StationIndex
        };
    }
}
