//-----------------------------------------------------------------------
// <copyright file="ServiceFabricEventJournal.cs" company="Microsoft Corporation">
//     Copyright (C) Microsoft Corporation. All rights reserved. 
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceFabricPersistence.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence
{
    public class ServiceFabricEventJournal : Actor<ServiceFabricEventJournalState>, IServiceFabricEventJournal
    {
        public override Task OnActivateAsync()
        {
            if (this.State == null)
            {
                this.State = new ServiceFabricEventJournalState();
            }

            ActorEventSource.Current.ActorMessage(this, "State initialized to {0}", this.State);
            return Task.FromResult(true);
        }

        public Task<List<JournalEntry>> GetMessagesAsync(long fromSequenceNr, long toSequenceNr, long max)
        {
            int m = max > int.MaxValue ? int.MaxValue : (int)max;

            ActorEventSource.Current.ActorMessage(this, "getMessageAsync {0}-{1}-{2}", fromSequenceNr, toSequenceNr, max);
            var messages = this
                .State
                .eventStore
                .Where(e => 
                e.Key >= fromSequenceNr && 
                e.Key <= toSequenceNr)
                .Take(m) //HACK: this will not work for large sets, on the other hand, fetching more would take an eternity...
                .Select(e => e.Value)
                .ToList();

            return Task.FromResult(messages);
        }

        public Task<long> GetHighestSequenceNrAsync()
        {
            if (State.eventStore.Count > 0)
                return Task.FromResult(this.State.eventStore.Keys.Max());
            else
                return Task.FromResult(0L);
        }

        public Task WriteMessageAsync(JournalEntry e)
        {
            this.State.eventStore.Add(e.SequenceNr, e);
            return Task.FromResult(true);
        }
        public Task DeleteMessagesToAsync(long toSequenceNr, bool isPermanent)
        {
                foreach (var e in this.State.eventStore.Where(o => o.Key <= toSequenceNr))
                {
                if (!isPermanent)
                    e.Value.IsDeleted = true;
                else
                    this.State.eventStore.Remove(e.Key);
                }

            return Task.FromResult(true);
        }

        public Task ClearAsync()
        {
            this.State = new ServiceFabricEventJournalState();
            return Task.FromResult(true);
        }
    }
}
