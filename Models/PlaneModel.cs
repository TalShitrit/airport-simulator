using Newtonsoft.Json;
using System;

namespace Models
{
    public class PlaneModel : IJsonable
    {
        public string PlaneID { get; set; }
        public int StationIndex { get; set; }
        public bool IsLanding { get; set; }
        public string StationFrom { get; set; }
        public string StationTo { get; set; }
        public DateTime Time { get; set; }

        public string GetJsonData() => JsonConvert.SerializeObject(this);
    }
}
