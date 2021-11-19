using Newtonsoft.Json;

namespace Models
{
    public class AirportStationModel :IJsonable
    {
        public string Id { get; set; }
        public int TimeToMove { get; set; }
        public bool IsClear { get; set; }
        public bool ReadyToExit { get; set; }

        public string GetJsonData() => JsonConvert.SerializeObject(this);
    }
}
