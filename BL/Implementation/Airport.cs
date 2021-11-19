using BL.API;
using DAL;
using DAL.API;
using DAL.Implementation;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BL.Implementation
{
    public class Airport : IAirport
    {
        private readonly List<IAirportStation> _stations;
        private  Queue<IPlane> _landingPlans;
        private  Queue<IPlane> _takeOffPlans;

        public Airport(bool loadStationData = true)
        {
            _stations = new List<IAirportStation>();
            _landingPlans = new Queue<IPlane>();
            _takeOffPlans = new Queue<IPlane>();

            LoadStation(loadStationData).Wait();
        }
   
        public IAirportStation GetStationById(string id) => GetAllStations().Single(s => s.Id == id);
        public List<IAirportStation> GetAllStations() => _stations;
        public void StationCleared(IStationRoute station)
        {
            if (station is LandingRoute)
            {
                if (_landingPlans.Count > 0)
                {
                    var plan = _landingPlans.Dequeue();
                    plan.MoveToNextStation();
                    MyOutPut.PlaneExitQueuePlan(plan);
                }
            }
            else
            {
                if (_takeOffPlans.Count > 0)
                {
                    var plan = _takeOffPlans.Dequeue();
                    plan.MoveToNextStation();
                    MyOutPut.PlaneExitQueuePlan(plan);
                }
            }
        }
        public void TryStart(IPlane plane)
        {
            if (plane.CanStartPlane())
                plane.MoveToNextStation();
            else
                AddToPlantTO(plane);
        }
        public bool TryTakeOutPlane(string planeId)
        {
            if (planeId is null) return false;
            foreach (var station in _stations)
                if (station.TryTakeOutPlane(planeId))
                {
                    MyOutPut.PlaneRemovedFromAirPort(planeId);
                    return true;
                }
            if (_landingPlans.Count(p => p.GetId == planeId) > 0)
            {
                _landingPlans = new Queue<IPlane>(_landingPlans.Where(pl => pl.GetId != planeId));
                MyOutPut.PlaneRemovedFromAirPort(planeId,true);
                return true;
            }
            if (_takeOffPlans.Count(p => p.GetId == planeId) > 0)
            {
                _takeOffPlans = new Queue<IPlane>(_takeOffPlans.Where(pl => pl.GetId != planeId));
                MyOutPut.PlaneRemovedFromAirPort(planeId, true);
                return true;
            }

            return false;
        }
        public void Refresh()
        {     
            AddPlanesInQueue();
        }

        private void AddToPlantTO(IPlane plane)
        {
            if (plane.IsLanding)
                _landingPlans.Enqueue(plane);
            else
                _takeOffPlans.Enqueue(plane);
            MyOutPut.PlaneEnterQueuePlan(plane);
        }
        private async Task LoadStation(bool loadStationData)
        {
            var res = await FileWorker.LoadJsonAsync<AirportStationModel>(Config.ConfigStationPath);
            foreach (var stationModel in res)
                _stations.Add(new AirportStation(stationModel, this));

            if (loadStationData)
            {
                AddPlanesInStations();
                AddPlanesInQueue();
            }
            else
            {
                FileWorker.ResetFile(Config.LandingQueueFilePath);
                FileWorker.ResetFile(Config.TakeOffQueueFilePath);
            }


        }
        private void AddPlanesInQueue()
        {
            IPlaneQueue planeQueue = new PlaneQueueFile();
            var landingQueue = FileWorker.ReadFileLines(Config.LandingQueueFilePath);
            var takeOffQueue = FileWorker.ReadFileLines(Config.TakeOffQueueFilePath);
            foreach (var landing in landingQueue)
            {
                if (!string.IsNullOrEmpty(landing))
                {
                    var data = planeQueue.PlaneExitQueue(true);
                    if (data != null)
                    {
                        var plane = GetPlaneFromModel(data);
                        if (plane != null)
                        {
                            if (plane.CanStartPlane())
                                plane.MoveToNextStation();
                            else
                                _landingPlans.Enqueue(plane);
                        }
                    }
                }
            }
            foreach (var takeOff in takeOffQueue)
            {
                if (!string.IsNullOrEmpty(takeOff))
                {
                    var data = planeQueue.PlaneExitQueue(false);
                    if (data != null)
                    {
                        var plane = GetPlaneFromModel(data);
                        if (plane != null)
                        {
                            if (plane.CanStartPlane())
                                plane.MoveToNextStation();
                            else
                                _takeOffPlans.Enqueue(plane);
                        }
                    }
                }
            }
        }
        private async void AddPlanesInStations()
        {
            var stationStatus = await FileWorker.LoadJsonAsync<StationStatusModel>(Config.StationStatusPath);
            foreach (var station in stationStatus)
            {
                if (station.PlaneId != null)
                {
                    var stationToAdd = _stations.Single(s => s.Id == station.StationId);
                    IStationRoute route;
                    if (station.IsPlanLanding)
                        route = new LandingRoute(this);
                    else
                        route = new TakeOffRoute(this);

                    IPlane p = new Plane(route, new AirportLogic(this), station.PlaneId);
                    p.StationIndex = station.StationIndex;
                    p.StationId = station.StationId;
                    TryStart(p);
                }
            }
        }
        private Plane GetPlaneFromModel(PlaneModel model)
        {
            if (model is null)
            {
                FileWorker.WriteToLog("GetPlaneFromModel got null model");
                return null;
            }
            IStationRoute route;
            if (model.IsLanding)
                route = new LandingRoute(this);
            else
                route = new TakeOffRoute(this);
            return new Plane(route, new AirportLogic(this), model.PlaneID);
        }

    }
}
