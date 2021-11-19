namespace BL.API
{
    public interface IAirportLogic :IRefreshable
    {
        void StartMove(IPlane plane);
        void MoveCompleted(IPlane plane);
    }
}
