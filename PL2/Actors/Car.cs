using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Akka.Actor;

namespace PL2.Actors
{
    public class Car : ReceiveActor
    {
        private IActorRef ParkPlace { get; set; }
        private string Name { get; }
        public Car (List<IActorRef> parkPlaces, string name) {
            Name = name;
            foreach (var actor in parkPlaces){ actor.Tell(Command.IsLotFree, Self); }
            Context
                .System
                .Scheduler
                .ScheduleTellOnce(
                TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(3, 5)),
                Self,
                Command.LeaveParking,
                Self
            );

            Receive<Command>(cmd =>
            {
                switch (cmd)
                {
                    case Command.ParkingLotIsFree:
                        Park();
                        break;
                    case Command.LeaveParking:
                        GoToAnotherParking();
                        break;
                }
            });
        }
        
        
        private void Park() {
            if (ParkPlace is null) {
                ParkPlace = Sender;
                LeaveParkingAfterDelay();
            } else {
                Sender.Tell(Command.AlreadyParked, Self);
            }
        }
        
        private void LeaveParkingAfterDelay()
        {
            var delay = TimeSpan.FromSeconds(RandomNumberGenerator.GetInt32(2, 4));
            Context
                .System
                .Scheduler
                .ScheduleTellOnce(delay, ParkPlace, Command.LeaveParking, Self);
        }
        
        private void GoToAnotherParking()
        {
            if (ParkPlace != null) return;
            Context.System.Stop(Self);
        }
    }
}