using BL;
using BL.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    public class FakeStationRoute : IStationRoute
    {
        private readonly IAirport _airport;
        private readonly List<IAirportStation> _stations;
        public FakeStationRoute(IAirport airport)
        {
            _airport = airport;
            _stations = new List<IAirportStation>();
              SetRoute();
        }

        public bool CanTakeNewPlane()
        {
            throw new NotImplementedException();
        }

        public bool MoveToNextStation(IPlane plane)
        {
            if (plane.StationIndex < _stations.Count)
            {
                //MyOutPut.PlaneMoveToNextStation(plane, _stations[plane.StationIndex]);
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
        { 
            _stations.Add(_airport.GetStationById("1"));
            //_stations.Add(_airport.Get("2"));
            //_stations.Add(_airport.Get("3"));
            //_stations.Add(_airport.Get("4"));
            //_stations.Add(_airport.Get("5"));
            //_stations.Add(_airport.Get("6"));
            //_stations.Add(_airport.Get("7"));
            //_stations.Add(_airport.Get("8"));
        }
    }
}
