// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the Apache 2 License.  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public readonly string SnapshotType;
        [DataMember]
        public readonly byte[] Snapshot;

        public SnapshotEntry(string persistenceId, long sequenceNr, DateTime timestamp, string snapshotType, byte[] snapshot)
        {
            PersistenceId = persistenceId;
            SequenceNr = sequenceNr;
            Timestamp = timestamp;
            SnapshotType = snapshotType;
            Snapshot = snapshot;
        }
    }

    public interface IServiceFabricSnapshotStore : IActor
    {
       Task<SnapshotEntry> SelectSnapshotAsync(long maxSequenceNr, DateTime maxTimeStamp);
        Task WriteSnapshotAsync(SnapshotEntry s);
        Task DeleteSnapshotAsync(long maxSequenceNr, DateTime maxTimeStamp);
        Task DeleteSnapshotManyAsync(long maxSequenceNr, DateTime maxTimeStamp);




    }
}
