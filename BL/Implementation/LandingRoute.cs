using BL.API;
using System;
using System.Collections.Generic;

namespace BL.Implementation
{
    public class LandingRoute : IStationRoute
    {
        private readonly IAirport _airport;
        private readonly List<IAirportStation> _stations;
        public LandingRoute(IAirport airport)
        {
            _airport = airport;
            _stations = new List<IAirportStation>();
            SetRoute();
        }

        public bool CanTakeNewPlane() => _airport.GetStationById("1").IsClear;

        public bool MoveToNextStation(IPlane plane)
        {
            if (plane.StationIndex == 1)
                _airport.StationCleared(this);

            if (plane.StationIndex == 6 && _stations.Count == 5)//load plane at last station
            {
                _stations.Add(_airport.GetStationById(plane.StationId));
                _stations[plane.StationIndex-1].MoveToStationQueue(plane);
                return true;
            }
            else
            if (plane.StationIndex == 5)
            {
                int st6 = _airport.GetStationById("6").GetWaitQueue();
                int st7 = _airport.GetStationById("7").GetWaitQueue();
                if (st6 > st7)
                    _stations.Add(_airport.GetStationById("7"));
                else
                    _stations.Add(_airport.GetStationById("6"));
            }
            if (plane.StationIndex < _stations.Count)
            {
                _stations[plane.StationIndex++].MoveToStationQueue(plane);
                return true;
            }
            if (plane.StationIndex == _stations.Count)
                return false;

            throw new Exception();
        }
        private void SetRoute()
        {
            _stations.Add(_airport.GetStationById("1"));
            _stations.Add(_airport.GetStationById("2"));
            _stations.Add(_airport.GetStationById("3"));
            _stations.Add(_airport.GetStationById("4"));
            _stations.Add(_airport.GetStationById("5"));
        }
    }
}
