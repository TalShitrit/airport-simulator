using BL.API;
using BL.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class LandingTests
    {
        private readonly int t = 1;// 1 second for moving specific distance
        [TestMethod]
        public void StationGetOccupied()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane = new Plane(new LandingRoute(airport), airportLogic, "1");
            AirportStation station = new AirportStation("1", t, airport);
            Assert.IsTrue(station.IsClear);
            station.MoveToStationQueue(plane);
            Thread.Sleep(20);
            Assert.IsFalse(station.IsClear);
        }
        [TestMethod]
        public void CantLand()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane = new Plane(new LandingRoute(airport), airportLogic, "1");
            Plane plane2 = new Plane(new LandingRoute(airport), airportLogic, "2");
            AirportStation station = new AirportStation("1", t, airport);
            Assert.IsTrue(station.IsClear);
            station.MoveToStationQueue(plane);
            Assert.IsFalse(station.MoveToStationQueue(plane2));
        }
        [TestMethod]
        public void StationGetCleared()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            IPlane plane = new Plane(new LandingRoute(airport), airportLogic, "1");
            IAirportStation station = airport.GetStationById("1");
            Assert.IsTrue(station.IsClear);
            var res = plane.MoveToNextStation();
            Thread.Sleep(20);
            //var res = station.MoveToStationQueue(plane);
            Assert.IsTrue(res); // did move
            Assert.IsFalse(station.IsClear); // station is Occupied
            Thread.Sleep((int)(station.TimeToMove * 1.3 * 1000));
            Assert.IsTrue(station.IsClear); // station is Cleared
        }
    }
}
