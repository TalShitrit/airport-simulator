using BL.API;
using BL.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace UnitTestProject
{
    [TestClass]
    public class TakeOffTests
    {
        private readonly int t = 1;// 1 second for moving specific distance
        [TestMethod]
        public void StationGetOccupied()
        {
            Airport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane = new Plane(new TakeOffRoute(airport), airportLogic, "1");
            AirportStation station = new AirportStation("1", t, airport);
            Assert.IsTrue(station.IsClear);
            station.MoveToStationQueue(plane);
            Assert.IsFalse(station.IsClear);
        }
        [TestMethod]
        public void CantTakeoff()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane = new Plane(new TakeOffRoute(airport), airportLogic, "1");
            Plane plane2 = new Plane(new TakeOffRoute(airport), airportLogic, "2");
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
            IPlane plane = new Plane(new TakeOffRoute(airport), airportLogic, "1");
            IAirportStation station6 = airport.GetStationById("6");
            IAirportStation station7 = airport.GetStationById("7");
            Assert.IsTrue(station6.IsClear && station7.IsClear);
            Assert.IsTrue(plane.MoveToNextStation()); // did move
            Assert.IsFalse(station6.IsClear && station7.IsClear); // station is Occupied
            Thread.Sleep((int)(t * 2.5 * 1000));
            Assert.IsTrue(station6.IsClear && station7.IsClear); // station is Cleared
        }
    }
}
