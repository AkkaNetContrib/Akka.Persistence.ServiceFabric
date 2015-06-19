// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License.  
using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                using (FabricRuntime fabricRuntime = FabricRuntime.Create())
                {
                    fabricRuntime.RegisterActor(typeof(ServiceFabricEventJournal));
                    fabricRuntime.RegisterActor(typeof(ServiceFabricSnapshotStore));

                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e);
                throw;
            }
        }
    }
}
