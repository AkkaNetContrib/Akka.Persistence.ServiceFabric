// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License. 

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence.Interfaces
{
    [DataContract]
    public class JournalEntry
    {
        [DataMember]
        public readonly string PersistenceId;
        [DataMember]
        public readonly long SequenceNr;
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public readonly string PayloadType;
        [DataMember]
        public readonly byte[] Payload;

        public JournalEntry(string persistenceId, long sequenceNr, bool isDeleted, string payloadType, byte[] payload)
        {
            PersistenceId = persistenceId;
            SequenceNr = sequenceNr;
            IsDeleted = isDeleted;
            PayloadType = payloadType;
            Payload = payload;
        }
    }

    public interface IServiceFabricEventJournal : IActor
    {
        Task<List<JournalEntry>> GetMessagesAsync(long fromSequenceNr, long toSequenceNr, long max);
        Task<long> GetHighestSequenceNrAsync();

        Task WriteMessageAsync(JournalEntry e);

        Task DeleteMessagesToAsync(long toSequenceNr, bool isPermanent);

        Task ClearAsync();

    }
}
