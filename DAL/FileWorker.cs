using DAL.Implementation;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL
{
    public class FileWorker
    {
        public static string[] ReadFileLines(string filename)
        {
            try
            {
                string _path = Path.Combine(Config.FilesPath, filename);
                if (File.Exists(_path))
                    return File.ReadAllLines(_path);
                else
                {
                    File.Create(_path).Close();
                    return File.ReadAllLines(_path);
                }
            }
            catch (Exception ex)
            {
                WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = ex,
                    FileName = filename,
                    Info = $"Faild to Read data at function ReadFileLines"
                });
                return null;
            }
        }
        public static string TakeFirstLine(string fileName)
        {
            Exception ex = null;
            for (int i = 0; i < Config.SleepRepite; i++)
            {
                try
                {
                    string path = Path.Combine(Config.FilesPath, fileName);
                    var data = File.ReadAllLines(path);
                    if (data is null || data.Length == 0)
                        return null;
                    var firstLine = data[0];
                    var toSave = data.ToList();
                    var res = toSave.Remove(firstLine);
                    if (res is false) throw new Exception();
                    File.WriteAllLines(path, toSave);
                    return firstLine;
                }
                catch (IOException e)
                {
                    ex = e;
                    Thread.Sleep(Config.SleepTime);
                }
            }
            WriteToLog(new LogError
            {
                ErrorTime = DateTime.Now,
                ExceptionThrown = ex,
                FileName = fileName,
                Info = $"Faild at function TakeFirstLine"
            });
            return null;
        }
        public static void ResetFile(string fileName)
        {
            try
            {
                string path = Path.Combine(Config.FilesPath, fileName);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    File.Create(path).Close();
                }
            }
            catch (Exception ex)
            {
                WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = ex,
                    FileName = fileName,
                    Info = $"Faild to ResetFile"
                });
            }
        }

 

        public static void WriteFileLine(string fileName, string data)
        {
            string path = Path.Combine(Config.FilesPath, fileName);
            if (File.Exists(path))
            {
                Exception ex = null;
                for (int i = 0; i < Config.SleepRepite; i++)
                {
                    try
                    {
                        using (var sw = File.AppendText(path))
                        {
                            sw.WriteLine(data);
                            return;
                        }
                    }
                    catch (IOException e)
                    {
                        ex = e;
                        Thread.Sleep(Config.SleepTime);
                    }
                }
                WriteToLog(new LogError
                {
                    ErrorTime = DateTime.Now,
                    ExceptionThrown = ex,
                    FileName = fileName,
                    Info = $"Faild to ResetFile"
                });
            }
            else
            {
                File.Create(path).Close();
            }
        }
        public static void WriteToLog(string info)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    string errorData = $"At time {DateTime.Now} {info}";
                    WriteFileLine(Config.LogPath, errorData);
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(2);
                }
            }
        }
        public static void WriteToLog(LogError dataError)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (dataError is null) throw new NullReferenceException();
                    string errorData = $"At time {dataError.ErrorTime} file: {dataError.FileName} with data {dataError.Info} throw error {dataError.ExceptionThrown.GetType().FullName}";
                    WriteFileLine(Config.LogPath, errorData);
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(2);
                }
            }
        }
        public async static Task SaveJsonAsync<T>(string fileName, List<T> data) where T : IJsonable
        {
            string path = Path.Combine(Config.FilesPath, fileName);
            if (!File.Exists(path))
            {
                if (!Directory.Exists(Config.FilesPath))
                    Directory.CreateDirectory(Config.FilesPath);
                File.Create(path).Close();
            }

            Exception ex = null;
            var stringData = JsonConvert.SerializeObject(data);
            for (int i = 0; i < Config.SleepRepite; i++)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(path))
                    {
                        await sw.WriteAsync(stringData);
                        return;
                    }
                }
                catch (IOException e)
                {
                    ex = e;
                    Thread.Sleep(Config.SleepTime);
                }
            }
            WriteToLog(new LogError
            {
                ErrorTime = DateTime.Now,
                ExceptionThrown = ex,
                FileName = fileName,
                Info = $"Faild to save data at function SaveJsonAsync"
            });
        }
        public async static Task<List<T>> LoadJsonAsync<T>(string fileName) where T : IJsonable
        {
            string path = Path.Combine(Config.FilesPath, fileName);
            if (File.Exists(path))
                return await LoadJsonFileAsync<T>(fileName, path);
            return await CreateAndLoadJsonAsync<T>(fileName, path);
        }

        private static async Task<List<T>> LoadJsonFileAsync<T>(string fileName, string path) where T : IJsonable
        {
            Exception ex = null;
            for (int i = 0; i < Config.SleepRepite; i++)
            {
                try
                {
                    string json;
                    using (StreamReader r = new StreamReader(path))
                    {
                        json = await r.ReadToEndAsync();
                    }
                    if (string.IsNullOrEmpty(json))
                        return await CreateAndLoadJsonAsync<T>(fileName, path);
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                catch (IOException e)
                {
                    ex = e;
                    Thread.Sleep(Config.SleepTime);
                }
            }
            WriteToLog(new LogError
            {
                ErrorTime = DateTime.Now,
                ExceptionThrown = ex,
                FileName = fileName,
                Info = $"Faild to load data at function LoadJsonAsync"
            });
            return null;
        }
        private static async Task<List<T>> CreateAndLoadJsonAsync<T>(string fileName, string path) where T : IJsonable
        {
            if (!Directory.Exists(Config.FilesPath))
                Directory.CreateDirectory(Config.FilesPath);

            if (fileName == Config.StationStatusPath)
                await StationStatusFiles.CreateDefValuesAsync();
            if (fileName == Config.ConfigStationPath)
            {
                List<AirportStationModel> defStation = new List<AirportStationModel>();
                for (int i = 1; i <= 8; i++)
                    defStation.Add(new AirportStationModel { Id = i.ToString(), ReadyToExit = true, IsClear = true, TimeToMove = 2 });
                await SaveJsonAsync(fileName, defStation);
            }
            try
            {
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
            }
            catch (IOException)
            {
                throw new Exception();
            }
        }
    }
}
