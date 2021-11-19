using Newtonsoft.Json;

namespace Models
{
    public class StationStatusModel : IJsonable
    {
        public string StationId { get; set; }
        public string PlaneId { get; set; }
        public int StationIndex { get; set; }
        public bool IsPlanLanding { get; set; }

        public string GetJsonData() => JsonConvert.SerializeObject(this);
    }
}
