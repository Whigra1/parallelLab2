using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Akka.Actor;

namespace PL2.Actors
{
    class ParkingLot: ReceiveActor
    {
        
        private List<IActorRef> _waitingCars = new();
        // private IActorRef _parkedCar;
        private bool isCarParked;
        private string Name { get; } = "";
        public ParkingLot(string name)
        {
            Name = name;
            Receive<Command>(r =>
            {
                switch (r)
                {
                    case Command.IsLotFree:
                        CheckForSpaceAndParkCarIfFree();
                        break;
                    case Command.AlreadyParked:
                        ReleasePlace();
                        SendNotificationThatParkingLotIsFree();
                        break;
                    case Command.LeaveParking:
                        ReleasePlace();
                        Console.WriteLine("Car leave parking " + Name);
                        SendNotificationThatParkingLotIsFree();
                        break;
                }
            });
        }
        private void CheckForSpaceAndParkCarIfFree() {
            if (!isCarParked) {
                isCarParked = true;
                Sender.Tell(Command.ParkingLotIsFree, Self);
            } else {
                _waitingCars.Add(Sender);
            }
        }

        private void ReleasePlace() {
            isCarParked = false;
        }

        private void SendNotificationThatParkingLotIsFree() {
            for (int i = 0; i < _waitingCars.Count; i++)
            {
                RepointableActorRef firstCar = (RepointableActorRef) _waitingCars[0];
                if (firstCar.IsTerminated) { // Check if car already go away
                    _waitingCars.Remove(firstCar);
                } else {
                    isCarParked = true;
                    firstCar.Tell(Command.ParkingLotIsFree, Self);
                    _waitingCars.Remove(firstCar);
                    return;
                }
            }
        }

    }
}