namespace BL.API
{
    public interface IPlane
    {
        string GetId { get; }
        int StationIndex { get; set; }
        string StationId { get; set; }
        bool MoveToNextStation();
        bool IsLanding { get; }
        bool CanStartPlane();
    }
}