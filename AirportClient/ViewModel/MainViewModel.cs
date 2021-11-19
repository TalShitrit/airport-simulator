using AirportClient.Models;
using DAL;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.AspNetCore.SignalR.Client;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AirportClient.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        private HubConnection connection;
        private string _planeIdToRemove;
        public string PlaneIdToRemove
        {
            get { return _planeIdToRemove; }
            set { _planeIdToRemove = value; }
        }

        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand TryRemovePlaneCommand { get; set; }

        public ObservableCollection<string> Changes { get; set; }
        public ObservableCollection<StationStatusModel> StationStatus { get; set; }
        public ObservableCollection<PlaneData> PlaneDatas { get; set; }

        public MainViewModel()
        {
            PlaneDatas = new ObservableCollection<PlaneData>();
            StationStatus = new ObservableCollection<StationStatusModel>();
            Changes = new ObservableCollection<string>();
            RefreshCommand = new RelayCommand(ActiveRefresh);
            TryRemovePlaneCommand = new RelayCommand(TryRemovePlane);
            InitStanion();
            InitializeSignalR();
        }

        private async void ActiveRefresh()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await connection.InvokeAsync("ActiveRefresh");
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(5);
                }
            }
            MessageBox.Show("connection failed");
        }
        private async void TryRemovePlane()
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await connection.InvokeAsync("TryRemovePlane", PlaneIdToRemove);
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(5);
                }
            }
            MessageBox.Show("connection failed");
        }
        private async void InitializeSignalR()
        {
            string url = Config.API + Config.Hub;//http://localhost:62537/airPlaneHub"
            try
            {
                connection = new HubConnectionBuilder()
                            .WithUrl(url)
                            .WithAutomaticReconnect()
                            .Build();
                connection.KeepAliveInterval = TimeSpan.FromMinutes(10);
                await connection.StartAsync();

                connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };

                connection.On<string>("Text", (text) =>
                {
                    MessageBox.Show(text);
                });
                connection.On<string, string>("PlaneEnterStation", (planeID, stationID) =>
                 {
                     PlaneEnterStation(planeID, stationID);
                 });
                connection.On<string, string>("PlaneExitStation", (planeID, stationID) =>
                {
                    PlaneExitStation(planeID, stationID);

                });
                connection.On<string, bool>("PlaneStart", (planeID, IsLanding) =>
                {
                    PlaneStart(planeID, IsLanding);

                });
                connection.On<string>("PlaneFinished", (planeID) =>
                {
                    PlaneFinished(planeID);
                });
                connection.On<List<StationStatusModel>>("Refresh", (stationStatusModels) =>
                {
                    Refresh(stationStatusModels);
                });
                connection.On<string>("PlaneRemovedFromAirPort", (planeId) =>
                {
                    PlaneRemovedFromAirPort(planeId);
                });


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void PlaneRemovedFromAirPort(string planeId)
        {
            MessageBox.Show($"plane: {planeId} removed from the airport");
            Changes.Insert(0, $"plane: {planeId} removed from the airport");
            var plane = PlaneDatas.FirstOrDefault(p => p.PlaneId == planeId);
            if (plane != null && plane.StationAt != null)
            {
                int index = int.Parse(plane.StationAt) - 1;
                StationStatus[index] = new StationStatusModel { StationId = plane.StationAt, PlaneId = null };
            }
            else
            {
                for (int i = 0; i < StationStatus.Count; i++)
                {
                    if (StationStatus[i].PlaneId == planeId)
                    {
                        StationStatus[i] = new StationStatusModel { StationId = plane.StationAt, PlaneId = null };
                        break;
                    }
                }
            }
            PlaneFinished(planeId);
        }
        private void InitStanion()
        {
            for (int i = 1; i < 9; i++)
                StationStatus.Add(new StationStatusModel { StationId = i.ToString(), PlaneId = "" }); ;
        }
        private void PlaneEnterStation(string planeID, string stationID)
        {
            Changes.Insert(0, $"plane: {planeID} enter station {stationID}");
            int index = int.Parse(stationID) - 1;
            StationStatus[index] = new StationStatusModel { StationId = stationID, PlaneId = planeID };
            var plane = PlaneDatas.FirstOrDefault(p => p.PlaneId == planeID);
            if (plane is null)
                PlaneDatas.Add(new PlaneData { PlaneId = planeID, StationAt = stationID });
            else
                plane.StationAt = stationID;
        }
        private void PlaneExitStation(string planeID, string stationID)
        {
            Changes.Insert(0, $"plane: {planeID} exit station {stationID}");
            int index = int.Parse(stationID) - 1;
            StationStatus[index] = new StationStatusModel { StationId = stationID, PlaneId = null };
        }
        private void PlaneStart(string planeID, bool IsLanding) => PlaneDatas.Add(new PlaneData { PlaneId = planeID, IsLanding = IsLanding });
        private void PlaneFinished(string planeID)
        {
            try
            {
                var plane = PlaneDatas.First(p => p.PlaneId == planeID);
                PlaneDatas.Remove(plane);
            }
            catch (Exception e)
            {
                FileWorker.WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = e,
                    Info = "at function PlaneFinished of MainViewModel"
                });
            }
        }
        private void Refresh(List<StationStatusModel> stationStatusModels)
        {
            StationStatus.Clear();
            foreach (var item in stationStatusModels)
                StationStatus.Add(item);
            PlaneDatas.Clear();
            foreach (var item in stationStatusModels)
                if (item.PlaneId != null)
                    PlaneDatas.Add(new PlaneData { IsLanding = item.IsPlanLanding, PlaneId = item.PlaneId });
        }
    }
}