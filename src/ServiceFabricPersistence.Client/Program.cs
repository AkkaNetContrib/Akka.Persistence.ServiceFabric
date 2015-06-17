// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License. 

using ServiceFabricPersistence.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceFabricPersistence.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var proxy = ActorProxy.Create<IServiceFabricEventJournal>(ActorId.NewId(), "fabric:/ServiceFabricEventJournalApplication");

            int count = 10;
            Console.WriteLine("Setting Count to in Actor {0}: {1}", proxy.GetActorId(), count);
            List<JournalEntry> p = proxy.GetMessagesAsync(1, long.MaxValue, long.MaxValue).Result;
//            proxy.SetCountAsync(count).Wait();

  //          Console.WriteLine("Count from Actor {1}: {0}", proxy.GetActorId(), proxy.GetCountAsync().Result);
        }
    }
}
