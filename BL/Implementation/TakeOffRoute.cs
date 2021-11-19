using BL.API;
using System;
using System.Collections.Generic;

namespace BL.Implementation
{
    public class TakeOffRoute : IStationRoute
    {
        private readonly IAirport _airport;
        private readonly List<IAirportStation> _stations;
        public TakeOffRoute(IAirport airport)
        {
            _airport = airport;
            _stations = new List<IAirportStation>();
        }
        public bool CanTakeNewPlane()
        {
            var st6 = _airport.GetStationById("6").IsClear;
            var st7 = _airport.GetStationById("7").GetWaitQueue();
            if (st6)
                return st7 < 2;
            return false;
        }

        public bool MoveToNextStation(IPlane plane)
        {

            if (plane.StationIndex == 1 && _stations.Count > 0)
                _airport.StationCleared(this);

            if (plane.StationIndex == 0)
            {
                var st1 = _airport.GetStationById("1").IsClear;
                var st2 = _airport.GetStationById("2").IsClear;
                var st3 = _airport.GetStationById("3").IsClear;

                int st6 = _airport.GetStationById("6").GetWaitQueue();
                int st7 = _airport.GetStationById("7").GetWaitQueue();
                if (st1 && st2 && st3)// no plug that way    
                {
                    if (st6 >= st7)
                        _stations.Add(_airport.GetStationById("7"));
                    else
                        _stations.Add(_airport.GetStationById("6"));
                }
                else
                    _stations.Add(_airport.GetStationById("7"));
                SetRoute();
            }
            if (plane.StationIndex > 0 && _stations.Count == 0)//loaded plane 
            {
                _stations.Add(_airport.GetStationById("7"));
                _stations.Add(_airport.GetStationById("8"));
                _stations.Add(_airport.GetStationById("4"));
            }
            if (plane.StationIndex < _stations.Count)
            {
                _stations[plane.StationIndex++].MoveToStationQueue(plane);
                return true;
            }
            if (plane.StationIndex == _stations.Count)
            {
                return false;
            }//finished
            throw new Exception();
        }
        private void SetRoute()
        { // basic stations
            _stations.Add(_airport.GetStationById("8"));
            _stations.Add(_airport.GetStationById("4"));
        }
    }
}


