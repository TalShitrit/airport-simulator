namespace BL.API
{
    public interface IStationRoute
    {
        bool MoveToNextStation(IPlane plane);
        bool CanTakeNewPlane();
    }
}
