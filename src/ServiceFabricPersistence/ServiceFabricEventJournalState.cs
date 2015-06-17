using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ServiceFabricPersistence.Interfaces;
using Microsoft.ServiceFabric;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence
{
    [DataContract]
    public class ServiceFabricEventJournalState
    {
        [DataMember]
        public SortedList<long, JournalEntry> eventStore;

        public ServiceFabricEventJournalState()
        {
            eventStore = new SortedList<long, JournalEntry>();
        }
    }
}