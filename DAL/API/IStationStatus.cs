using Models;

namespace DAL.API
{
    public interface IStationStatus
    {
        void EmptyStation(string stationId);
        void EmptyStationByPlaneId(string planeId);
        void PlaneEnterStation(StationStatusModel stationStatus);
    }
}
