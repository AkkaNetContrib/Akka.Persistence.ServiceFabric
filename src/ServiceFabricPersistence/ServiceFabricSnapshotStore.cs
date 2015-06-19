// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License.  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceFabricPersistence.Interfaces;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence
{
    public class ServiceFabricSnapshotStore : Actor<ServiceFabricSnapshotStoreState>, IServiceFabricSnapshotStore
    {
        public override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new ServiceFabricSnapshotStoreState();
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }
        public Task<SnapshotEntry> SelectSnapshotAsync(long maxSequenceNr, DateTime maxTimeStamp)
        {
            IEnumerable<KeyValuePair<long, SnapshotEntry>> snapshots = State.snapshotStore;

            ActorEventSource.Current.ActorMessage(this, "selectSnapshotAsync {0}-{1}", maxSequenceNr, maxTimeStamp);
            if (maxSequenceNr > 0 && maxSequenceNr < long.MaxValue)
            {
                snapshots = from e in this.State.snapshotStore
                             where e.Key <= maxSequenceNr
                            select e;
            }
             
            if(maxTimeStamp > DateTime.MinValue && maxTimeStamp < DateTime.MaxValue)
            {
                snapshots = from e in snapshots
                            where e.Value.Timestamp == maxTimeStamp
                            select e;
            }
            //TODO: Double-check selection criteria
            var snapshot = snapshots.ToList<KeyValuePair<long, SnapshotEntry>>();

            var retValue = snapshot.Any() ? snapshot.Last().Value : null;
    
            return Task.FromResult(retValue);
        }

        public Task WriteSnapshotAsync(SnapshotEntry s)
        {
            ActorEventSource.Current.ActorMessage(this, "writeSnapshot {0}-{1}", s.SequenceNr, s.Timestamp);
            State.snapshotStore.Add(s.SequenceNr, s);

            return Task.FromResult(true);
        }

        public Task DeleteSnapshotAsync(long maxSequenceNr, DateTime maxTimeStamp)
        {
            IEnumerable<KeyValuePair<long, SnapshotEntry>> snapshots = State.snapshotStore;

            ActorEventSource.Current.ActorMessage(this, "deleteSnapshot {0}-{1}", maxSequenceNr, maxTimeStamp);

            ActorEventSource.Current.ActorMessage(this, "DeleteSnapshot {0}-{1}-{2}", maxSequenceNr, maxTimeStamp);
            if (maxSequenceNr > 0 && maxSequenceNr < long.MaxValue)
            {
                snapshots = from e in this.State.snapshotStore
                            where e.Key <= maxSequenceNr
                            select e;
            }

            if (maxTimeStamp > DateTime.MinValue && maxTimeStamp < DateTime.MaxValue)
            {
                snapshots = from e in snapshots
                               where e.Value.Timestamp == maxTimeStamp
                            select e;
            }
            foreach (var s in snapshots)
                State.snapshotStore.Remove(s.Key);

            return Task.FromResult(true);

        }
        public Task DeleteSnapshotManyAsync(long maxSequenceNr, DateTime maxTimeStamp)
        {
            ActorEventSource.Current.ActorMessage(this, "DeleteSnapshotMany {0}-{1}", maxSequenceNr, maxTimeStamp);

            if (maxSequenceNr > 0 && maxSequenceNr < long.MaxValue)
            {
                var snapshot = from e in this.State.snapshotStore
                               where e.Key == maxSequenceNr
                               select e;
                State.snapshotStore.Remove(snapshot.First().Key);
            }

            if (maxTimeStamp > DateTime.MinValue && maxTimeStamp < DateTime.MaxValue)
            {
                var snapshot = from e in this.State.snapshotStore
                               where e.Value.Timestamp == maxTimeStamp
                               select e;
                State.snapshotStore.Remove(snapshot.First().Key);
            }

            return Task.FromResult(true);

        }


    }
}
