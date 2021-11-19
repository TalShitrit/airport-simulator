using DAL.API;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Implementation
{
    public class StationStatusFiles : IStationStatus
    {
        private readonly Queue<Action> _changeQueue = new Queue<Action>();
        private readonly object _lockTryActive = new object();

        public static Task CreateDefValuesAsync()
        {
            return Task.Run(() =>
             {
                 string path = Path.Combine(Config.FilesPath, Config.StationStatusPath);
                 var stationConfig = FileWorker.LoadJsonAsync<AirportStationModel>(Config.ConfigStationPath).GetAwaiter().GetResult();
                 List<StationStatusModel> stationStatuses = new List<StationStatusModel>();

                 foreach (var item in stationConfig)
                     stationStatuses.Add(new StationStatusModel { StationId = item.Id, PlaneId = null });
                 var data = JsonConvert.SerializeObject(stationStatuses);

                 for (int i = 0; i < 5; i++)
                 {
                     try
                     {
                         using (StreamWriter sw = File.CreateText(path))
                         {
                             sw.WriteLine(data);
                             return;
                         }
                     }
                     catch (IOException)
                     {
                         Thread.Sleep(2);
                     }
                 }
                 throw new Exception();

             });

        }
        public void PlaneEnterStation(StationStatusModel stationStatus)
        {
            TryActive(() =>
            {
                var data = GetStatusAsync().GetAwaiter().GetResult();
                var station = data.Single(s => s.StationId == stationStatus.StationId);
                station.PlaneId = stationStatus.PlaneId;
                station.StationIndex = stationStatus.StationIndex;
                station.IsPlanLanding = stationStatus.IsPlanLanding;
                SaveStatusAsync(data).Wait();
                FinishedAction();
            });

        }
        public void EmptyStation(string stationId)
        {
            TryActive(() =>
            {
                var data = GetStatusAsync().GetAwaiter().GetResult();
                var station = data.Single(s => s.StationId == stationId);
                station.PlaneId = null;
                station.StationIndex = -1;
                station.IsPlanLanding = false;
                SaveStatusAsync(data).Wait();
                FinishedAction();
            });

        }
        public void EmptyStationByPlaneId(string planeId)
        {
            TryActive(() =>
            {
                var data = GetStatusAsync().GetAwaiter().GetResult();
                var station = data.Single(s => s.PlaneId == planeId);
                station.PlaneId = null;
                station.StationIndex = -1;
                station.IsPlanLanding = false;
                SaveStatusAsync(data).Wait();
                FinishedAction();
            });
        }

        private void TryActive(Action action)
        {
            lock (_lockTryActive)
            {
                if (_changeQueue.Count == 0)
                    action.Invoke();
                else _changeQueue.Enqueue(action);
                return;
            }
        }
        private void FinishedAction()
        {
            if (_changeQueue.Count > 0)
                Task.Run(() => _changeQueue.Dequeue().Invoke());
        }
        private Task<List<StationStatusModel>> GetStatusAsync()
        {
            return Task.Run(() =>
            {
                var data = FileWorker.LoadJsonAsync<StationStatusModel>(Config.StationStatusPath);
                if (data is null) CreateDefValuesAsync().GetAwaiter().GetResult();
                data = FileWorker.LoadJsonAsync<StationStatusModel>(Config.StationStatusPath);
                if (data is null) throw new Exception();
                return data;
            });

        }
        private Task SaveStatusAsync(List<StationStatusModel> data) =>
            Task.Run(async () => await FileWorker.SaveJsonAsync(Config.StationStatusPath, data));
    }
}
