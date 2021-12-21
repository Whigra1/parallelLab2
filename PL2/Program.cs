using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using Akka.Actor;
using PL2.Actors;

namespace PL2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ActorSystem actorSystem = ActorSystem.Create("mySystem");
            List<IActorRef> actorRefs = new()
            {
                actorSystem.ActorOf(Props.Create(() => new ParkingLot("Parking lot 1"))),
                actorSystem.ActorOf(Props.Create(() => new ParkingLot("Parking lot 2"))),
                actorSystem.ActorOf(Props.Create(() => new ParkingLot("Parking lot 3")))
            };
            Start(actorSystem, actorRefs);
        }

        public static Thread Start(ActorSystem system, List<IActorRef> cars)
        {
            void Start () {
                for (long i = 0; i < long.MaxValue; i++)
                {
                    try
                    {
                        Thread.Sleep(RandomNumberGenerator.GetInt32(500, 1500));
                        system.ActorOf(Props.Create(() => new Car(cars, "car" + i)));
                    }
                    catch (Exception e)
                    {
                    }  
                }
            }

            var thread = new Thread(Start);
            thread.Start();
            return thread;
        }

    
    }
}