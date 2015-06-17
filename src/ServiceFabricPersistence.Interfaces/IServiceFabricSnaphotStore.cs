// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License. 

using System;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.ServiceFabric.Actors;

namespace ServiceFabricPersistence.Interfaces
{
    [DataContract]
    public class SnapshotEntry
    {
        [DataMember]
        public readonly string PersistenceId;
        [DataMember]
        public readonly long SequenceNr;
        [DataMember]
        public DateTime Timestamp{ get; set; }
        [DataMember]
        public readonly string PayloadType;
        [DataMember]
        public readonly byte[] Payload;

        public SnapshotEntry(string persistenceId, long sequenceNr, DateTime timestamp, string payloadType, byte[] payload)
        {
            PersistenceId = persistenceId;
            SequenceNr = sequenceNr;
            Timestamp = timestamp;
            PayloadType = payloadType;
            Payload = payload;
        }
    }

    public interface IServiceFabricSnaphotStore : IActor
    {
       Task<SnapshotEntry> selectSnapshotAsync(long maxSequenceNr, DateTime maxTimeStamp);


    }
}
