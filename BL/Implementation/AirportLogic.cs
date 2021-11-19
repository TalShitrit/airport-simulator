using BL.API;

namespace BL.Implementation
{
    public class AirportLogic : IAirportLogic
    {

        private readonly IAirport _airport;
        public AirportLogic(IAirport airport) => _airport = airport;

        public void StartMove(IPlane plane) => _airport.TryStart(plane);
        public void MoveCompleted(IPlane plane)
        {
            _airport.GetStationById(plane.StationId).StationCleared();
            MyOutPut.PlaneFinished(plane);
        }
        public void Refresh()
        {
            foreach (var item in _airport.GetAllStations())
                item.TryEnterStation();
        }
    }
}
