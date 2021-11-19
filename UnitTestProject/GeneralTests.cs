using BL;
using BL.API;
using BL.Implementation;
using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;


namespace UnitTestProject
{
    [TestClass]
    public class GeneralTests
    {
        private readonly int t = 1;
        [TestMethod]
        public void NoMoreStation()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            //FakeStationRoute has only one route
            Plane plane = new Plane(new FakeStationRoute(airport), airportLogic, "1");
            var didmoved = plane.MoveToNextStation();
            Assert.IsTrue(didmoved);
            Thread.Sleep((int)(t * 1.1 * 1000));
            var didntmoved = plane.MoveToNextStation();
            Assert.IsFalse(didntmoved);
        }
        [TestMethod]
        public void PlaneWaitToEnter()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane1 = new Plane(new LandingRoute(airport), airportLogic, "1");
            Plane plane2 = new Plane(new LandingRoute(airport), airportLogic, "2");
            var didmoved1 = plane1.MoveToNextStation();
            plane2.MoveToNextStation();// just enter queue
            Assert.IsTrue(didmoved1);
            Assert.IsTrue(plane2.StationId is null);
        }
        [TestMethod]
        public void PlaneEnterFiles()
        {
            IAirport airport = new Airport(false);
            Thread.Sleep(20);
            IAirportLogic airportLogic = new AirportLogic(airport);
            Plane plane = new Plane(new LandingRoute(airport), airportLogic, "1");
            MyOutPut.StartStaticCTor();
            Thread.Sleep(50);
            var before = FileWorker.ReadFileLines(Config.PlaneChangePath).Length;
            Assert.IsTrue(plane.MoveToNextStation());
            Thread.Sleep(100);
            var after = FileWorker.ReadFileLines(Config.PlaneChangePath).Length;
            Assert.IsTrue(before < after);
        }
    }
}
