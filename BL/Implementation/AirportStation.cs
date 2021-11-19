using BL.API;
using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BL.Implementation
{
    public class AirportStation : AirportStationModel, IAirportStation
    {
        private readonly object _lockEnterStation = new object();
        private IPlane _currentPlane;
        private readonly IAirport _airport;
        private Queue<IPlane> _planQueue;//consider use concarate queue

        public AirportStation(AirportStationModel model, IAirport airport)
        {
            Id = model.Id;
            IsClear = model.IsClear;
            ReadyToExit = model.ReadyToExit;
            TimeToMove = model.TimeToMove;
            _airport = airport;
            _planQueue = new Queue<IPlane>();
        }
        public AirportStation(string id, int timeToMove, IAirport airport)
        {
            Id = id;
            TimeToMove = timeToMove;
            _airport = airport;
            _planQueue = new Queue<IPlane>();
            IsClear = true;
        }

        public void ExitStation()
        {
            _currentPlane = null;
            IsClear = true;
            ReadyToExit = true;
            TryEnterStation();
        }
        public bool MoveToStationQueue(IPlane plane)
        {
            if (plane != null)
                _planQueue.Enqueue(plane);
            return TryEnterStation();// if cleared start moving
        }
        public bool TryEnterStation()
        {
            lock (_lockEnterStation)
            {
                try
                {
                    if (IsClear && _planQueue.Count > 0)
                        return EnterStation();
                }
                catch (Exception e)
                {
                    if (_currentPlane is null)
                        FileWorker.WriteToLog("at TryEnterStation _currentPlane is null");

                    else
                        FileWorker.WriteToLog(new LogError
                        { ErrorTime = DateTime.Now, ExceptionThrown = e, Info = "thrown at functin TryEnterStation" });
                    ExitStation();
                }
            }
            return false;
        }
        public int GetWaitQueue() => _planQueue.Count;
        public void StationCleared()
        {
            if (_currentPlane != null)
            {
                MyOutPut.PlaneExitStation(_currentPlane, this);
                ExitStation();
            }
        }
        public Queue<IPlane> GetQueue() => _planQueue;
        public bool TryTakeOutPlane(string planeId)
        {
            if (_currentPlane != null && _currentPlane.GetId == planeId)
            {
                ExitStation();
                return true;
            }
            if (_planQueue.Count(p => p.GetId == planeId) > 0)
            {
                _planQueue = new Queue<IPlane>(_planQueue.Where(pl => pl.GetId != planeId));
                return true;
            }

            return false;
        }

        private bool EnterStation()
        {
            _currentPlane = _planQueue.Dequeue();
            if (_currentPlane is null)
            {
                FileWorker.WriteToLog("at TryEnterStation _currentPlane is null");
                return false;
            }
            if (_currentPlane.StationId != null)
                _airport.GetStationById(_currentPlane.StationId).StationCleared();//take out from last station

            _currentPlane.StationId = Id;
            IsClear = false;
            ReadyToExit = false;
            Task.Run(() =>
            {
                MyOutPut.PlaneEnterStation(_currentPlane, this);
                Thread.Sleep(1000 * TimeToMove);
                ReadyToExit = true;
                if (_currentPlane != null)
                    _currentPlane.MoveToNextStation();
            });
            return true;
        }
    }
}
