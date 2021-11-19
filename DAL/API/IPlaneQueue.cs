using Models;

namespace DAL.API
{
    public interface IPlaneQueue
    {
        void PlaneEnterQueue(PlaneModel planeModel);
        PlaneModel PlaneExitQueue(bool isLanding);
    }
}
