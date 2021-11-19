using BL.API;

namespace BL.Implementation
{
    public class Plane : IPlane
    {
        private readonly IStationRoute _stationsRoute;
        private readonly IAirportLogic airportLogic;
        private readonly string Id;
        public bool IsLanding { get; private set; }
        public string GetId => Id;
        public int StationIndex { get; set; }
        public string StationId { get; set; }

        public bool CanStartPlane() => _stationsRoute.CanTakeNewPlane();
        

        public Plane(IStationRoute stationRoute, IAirportLogic airportLogic, string id)
        {
            _stationsRoute = stationRoute;
            if (_stationsRoute is LandingRoute)
                IsLanding = true;
            this.airportLogic = airportLogic;
            Id = id;
        }


        public bool MoveToNextStation()
        {
            var res = _stationsRoute.MoveToNextStation(this);
            if (res) return true;
            airportLogic.MoveCompleted(this);
            return false;
        }
    }
}
