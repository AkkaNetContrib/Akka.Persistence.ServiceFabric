using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Akka.Persistence.Snapshot;
using ServiceFabricPersistence.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace Akka.Persistence.ServiceFabric.Snapshot
{

        /// <summary>
        /// Actor used for storing incoming snapshots into persistent snapshot store backed by SQL Server database.
        /// </summary>
        public class ServiceFabricSnapshotStore : SnapshotStore
        {
            private readonly ServiceFabricPersistenceExtension _extension;
            private readonly string _servicefabricServiceUri;
            private readonly Akka.Serialization.Serialization _serialization;

            protected readonly LinkedList<CancellationTokenSource> PendingOperations;

            public ServiceFabricSnapshotStore()
            {
                _extension = ServiceFabricPersistence.Instance.Apply(Context.System);
                _serialization = Context.System.Serialization;

                var settings = _extension.SnapshotStoreSettings;
                _servicefabricServiceUri = settings.servicefabricServiceUri;
                PendingOperations = new LinkedList<CancellationTokenSource>();


            }

            /// <summary>
            /// Query builder used to convert snapshot store related operations into corresponding SQL queries.
            /// </summary>

            /// <summary>
            /// Query mapper used to map SQL query results into snapshots.
            /// </summary>

            protected override void PreStart()
            {
                base.PreStart();
            }

            protected override void PostStop()
            {
                base.PostStop();

                // stop all operations executed in the background
                var node = PendingOperations.First;
                while (node != null)
                {
                    var curr = node;
                    node = node.Next;

                    curr.Value.Cancel();
                    PendingOperations.Remove(curr);
                }

            }

            public SelectedSnapshot Map(SnapshotEntry e)
            {
                if (e == null)
                    return null;
                var persistenceId = e.PersistenceId;
                var sequenceNr = e.SequenceNr;
                var timestamp = e.Timestamp;

                var metadata = new SnapshotMetadata(persistenceId, sequenceNr, timestamp);

                var type = Type.GetType(e.SnapshotType, true);
                var serializer = _serialization.FindSerializerForType(type);
                var binary = e.Snapshot;

                var snapshot = serializer.FromBinary(binary, type);


                return new SelectedSnapshot(metadata, snapshot);
            }

            protected override Task<SelectedSnapshot> LoadAsync(string persistenceId, SnapshotSelectionCriteria criteria)
            {

                var proxy = ActorProxy.Create<IServiceFabricSnapshotStore>(new ActorId(persistenceId), _servicefabricServiceUri);


                return Task.FromResult(Map(proxy.SelectSnapshotAsync(criteria.MaxSequenceNr, criteria.MaxTimeStamp).Result));
            }

            protected override Task SaveAsync(SnapshotMetadata metadata, object snapshot)
            {
                var entry = ToSnapshotEntry(metadata, snapshot);
                var proxy = ActorProxy.Create<IServiceFabricSnapshotStore>(new ActorId(metadata.PersistenceId), _servicefabricServiceUri);

                return proxy.WriteSnapshotAsync(entry);
                
            }

            protected override void Saved(SnapshotMetadata metadata) { }

            protected override void Delete(SnapshotMetadata metadata)
            {
                var proxy = ActorProxy.Create<IServiceFabricSnapshotStore>(new ActorId(metadata.PersistenceId), _servicefabricServiceUri);

                proxy.DeleteSnapshotAsync(metadata.SequenceNr, metadata.Timestamp);

            }

            protected override void Delete(string persistenceId, SnapshotSelectionCriteria criteria)
            {
                var proxy = ActorProxy.Create<IServiceFabricSnapshotStore>(new ActorId(persistenceId), _servicefabricServiceUri);

                proxy.DeleteSnapshotManyAsync(criteria.MaxSequenceNr, criteria.MaxTimeStamp);
            }


            private CancellationTokenSource GetCancellationTokenSource()
            {
                var source = new CancellationTokenSource();
                PendingOperations.AddLast(source);
                return source;
            }

            private SnapshotEntry ToSnapshotEntry(SnapshotMetadata metadata, object snapshot)
            {
                var snapshotType = snapshot.GetType();
                var serializer = Context.System.Serialization.FindSerializerForType(snapshotType);

                var binary = serializer.ToBinary(snapshot);

                return new SnapshotEntry(
                    persistenceId: metadata.PersistenceId,
                    sequenceNr: metadata.SequenceNr,
                    timestamp: metadata.Timestamp,
                    snapshotType: snapshotType.QualifiedTypeName(),
                    snapshot: binary);
            }
        }
    }
