using Newtonsoft.Json;

namespace Models
{
    public class ConfigModel :IJsonable
    {
        public int SleepTime { get; set; }
        public int SleepRepite { get; set; }
        public string ConfigStationPath { get; set; }
        public string LogPath { get; set; }
        public string StationStatusPath { get; set; }
        public string API { get; set; }
        public string Hub { get; set; }
        public string LandingQueueFilePath { get; set; }
        public string TakeOffQueueFilePath { get; set; }
        public string PlaneChangePath { get; set; }

        public string GetJsonData() => JsonConvert.SerializeObject(this);
    }
}
