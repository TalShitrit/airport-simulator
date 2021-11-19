using System.Collections.Generic;

namespace BL.API
{
    public interface IAirport : IRefreshable
    {
        IAirportStation GetStationById(string id);
        List<IAirportStation> GetAllStations();
        void StationCleared(IStationRoute station);
        void TryStart(IPlane plane);
        bool TryTakeOutPlane(string planeId);
    }
}
