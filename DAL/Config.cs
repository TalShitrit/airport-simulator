using Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DAL
{
    public class Config
    {

        static Config()
        {
            ConfigModel data = null;
            //data=LoadConfig();
            if (data != null)
            {
                SleepTime = data.SleepTime;
                SleepRepite = data.SleepRepite;
                ConfigStationPath = data.ConfigStationPath;
                LogPath = data.LogPath;
                PlaneChangePath = data.PlaneChangePath;
                StationStatusPath = data.StationStatusPath;
                API = data.API;
                Hub = data.Hub;
                LandingQueueFilePath = data.LandingQueueFilePath;
                TakeOffQueueFilePath = data.TakeOffQueueFilePath;
            }
        }

        private static ConfigModel LoadConfig()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", "config.json");
            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<ConfigModel>(json);
            }
            catch (Exception e)
            {
                FileWorker.WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = e,
                    Info = "While trying to read config.json"
                });
                return null;
            }
        }
        public static readonly int SleepTime = 4;
        public static readonly int SleepRepite = 7;
        public static readonly string ConfigStationPath = "ConfigStation.json";
        public static readonly string LogPath = "LogError.txt";
        public static readonly string PlaneChangePath = "PlaneChange.txt";
        public static readonly string StationStatusPath = "StationStatus.json";
        public static readonly string API = @"http://localhost:62537";
        public static readonly string Hub = @"/airPlaneHub";
        public static readonly string LandingQueueFilePath = "LandingQueue.txt";
        public static readonly string TakeOffQueueFilePath = "TakeOffQueue.txt";
        public static readonly string FilesPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
    }
}
