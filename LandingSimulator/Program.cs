using System;
using System.Net.Http;
using System.Threading;

namespace LandingSimulator
{
    class Program
    {
        private readonly static string _apiPath = "http://localhost:62537/api/airport";
        private readonly static string _takeOff = "/takeOff";
        private readonly static string _landing = "/landing";
        private readonly static int _sleep = 1500;
        private readonly static int _planeNumber = 50;
        private readonly static HttpClient _httpClient = new HttpClient();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TimerSimulator();
        }

        private static void TimerSimulator()
        {
            Random rnd = new Random();
            int number;
            for (int i = 0; i < _planeNumber; i++)
            {
                number = rnd.Next(10);
                if (number % 2 == 0)
                    StartLanding();
                else
                    StartTakeOff();
                Thread.Sleep(_sleep);
            }
        }
        private static async void StartLanding()
        {
            try
            {
                var res = _httpClient.GetAsync(_apiPath + _landing).Result.Content;
                var content = await res.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private static async void StartTakeOff()
        {
            try
            {
                var res = _httpClient.GetAsync(_apiPath + _takeOff).Result.Content;
                var content = await res.ReadAsStringAsync();
                Console.WriteLine(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
