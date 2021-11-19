using Models;
using System.Threading.Tasks;

namespace DAL.API
{
    public interface IPlaneChange
    {
        Task StartAsync(PlaneModel plane);
        Task FinishAsync(PlaneModel plane);
        Task EnterStationAsync(PlaneModel plane);
        Task ExitStationAsync(PlaneModel plane);
        Task PlaneRemovedFromAirPort(string planeId);
        void ResetFile();
    }
}
