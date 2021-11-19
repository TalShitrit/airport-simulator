using DAL.API;
using Models;
using Newtonsoft.Json;

namespace DAL.Implementation
{
    public class PlaneQueueFile : IPlaneQueue
    {
        public void PlaneEnterQueue(PlaneModel planeModel)
        {
            if (planeModel.IsLanding)
                FileWorker.WriteFileLine(Config.LandingQueueFilePath, planeModel.GetJsonData());
            else
                FileWorker.WriteFileLine(Config.TakeOffQueueFilePath, planeModel.GetJsonData());
        }
        public PlaneModel PlaneExitQueue(bool isLanding)
        {
            string data;
            if (isLanding)
                data = FileWorker.TakeFirstLine(Config.LandingQueueFilePath);
            else
                data = FileWorker.TakeFirstLine(Config.TakeOffQueueFilePath);
            if (data is null) return null;
            return JsonConvert.DeserializeObject<PlaneModel>(data);
        }
    }
}
