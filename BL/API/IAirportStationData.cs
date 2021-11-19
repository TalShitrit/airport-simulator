using System.Collections.Generic;

namespace BL.API
{
    public interface IAirportStationData
    {
        string Id { get; }
        int TimeToMove { get; }
        bool IsClear { get; }
        int GetWaitQueue();
        Queue<IPlane> GetQueue();
    }
}