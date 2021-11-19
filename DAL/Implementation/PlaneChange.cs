using DAL.API;
using Models;
using System;
using System.Threading.Tasks;

namespace DAL.Implementation
{
    public class PlaneChange : IPlaneChange
    {
        private readonly IStationStatus _stationStatus;
        public PlaneChange() => _stationStatus = new StationStatusFiles();

        public Task EnterStationAsync(PlaneModel plane)
        {
            return Task.Run(() =>
             {
                 string data = $"plane: {plane.PlaneID} enter station {plane.StationTo} at {plane.Time}";
                 FileWorker.WriteFileLine(Config.PlaneChangePath, data);
                 _stationStatus.PlaneEnterStation(
                     new StationStatusModel
                     {
                         StationId = plane.StationTo,
                         PlaneId = plane.PlaneID,
                         IsPlanLanding = plane.IsLanding,
                         StationIndex = plane.StationIndex
                     });
             });

        }
        public Task ExitStationAsync(PlaneModel plane)
        {
            return Task.Run(() =>
            {
                string data = $"plane: {plane.PlaneID} exit station {plane.StationTo} at {plane.Time}";
                _stationStatus.EmptyStation(plane.StationFrom);
                FileWorker.WriteFileLine(Config.PlaneChangePath, data);
            });

        }
        public Task FinishAsync(PlaneModel plane)
        {
            return Task.Run(() =>
            {
                string data = $"plane: {plane.PlaneID} finished at {plane.Time}";
                FileWorker.WriteFileLine(Config.PlaneChangePath, data);
            });
        }
        public Task StartAsync(PlaneModel plane)
        {
            return Task.Run(() =>
            {
                string data = $"plane: {plane.PlaneID} start at {plane.Time}";
                FileWorker.WriteFileLine(Config.PlaneChangePath, data);
            });
        }
        public Task PlaneRemovedFromAirPort(string planeId)
        {
            return Task.Run(() =>
            {
                string data = $"plane: {planeId} removed at {DateTime.Now}";
                FileWorker.WriteFileLine(Config.PlaneChangePath, data);
            });
        }
        public void ResetFile() => FileWorker.ResetFile(Config.PlaneChangePath);
    }
}
