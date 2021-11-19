namespace BL.API
{
    public interface IAirportStationActions
    {
        bool MoveToStationQueue(IPlane plane);
        bool TryEnterStation();
        void ExitStation();
        void StationCleared();
    }
}