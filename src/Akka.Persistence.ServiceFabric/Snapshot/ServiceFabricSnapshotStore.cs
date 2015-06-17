//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Threading;
//using System.Threading.Tasks;
//using Akka.Persistence.Snapshot;
//using ServiceFabricPersistence.Interfaces;
//using Microsoft.ServiceFabric.Actors;

//namespace Akka.Persistence.ServiceFabric.Snapshot
//{

//        /// <summary>
//        /// Actor used for storing incoming snapshots into persistent snapshot store backed by SQL Server database.
//        /// </summary>
//        public class ServiceFabricSnapshotStore : SnapshotStore
//        {
//            private readonly ServiceFabricPersistenceExtension _extension;
//            private readonly string _servicefabricServiceUri;
//            private readonly Akka.Serialization.Serialization _serialization;

//            protected readonly LinkedList<CancellationTokenSource> PendingOperations;

//            public ServiceFabricSnapshotStore()
//            {
//                _extension = ServiceFabricPersistence.Instance.Apply(Context.System);
//                _serialization = Context.System.Serialization;

//                var settings = _extension.SnapshotStoreSettings;
//                _servicefabricServiceUri = settings.servicefabricServiceUri;
//                PendingOperations = new LinkedList<CancellationTokenSource>();


//            }

//            /// <summary>
//            /// Query builder used to convert snapshot store related operations into corresponding SQL queries.
//            /// </summary>

//            /// <summary>
//            /// Query mapper used to map SQL query results into snapshots.
//            /// </summary>

//            protected override void PreStart()
//            {
//                base.PreStart();
//            }

//            protected override void PostStop()
//            {
//                base.PostStop();

//                // stop all operations executed in the background
//                var node = PendingOperations.First;
//                while (node != null)
//                {
//                    var curr = node;
//                    node = node.Next;

//                    curr.Value.Cancel();
//                    PendingOperations.Remove(curr);
//                }

//            }

//            public SelectedSnapshot Map(SnapshotEntry e)
//            {
//                var persistenceId = e.PersistenceId;
//                var sequenceNr = e.SequenceNr;
//                var timestamp = e.Timestamp;

//                var metadata = new SnapshotMetadata(persistenceId, sequenceNr, timestamp);

//                var type = Type.GetType(e.PayloadType, true);
//                var serializer = _serialization.FindSerializerForType(type);
//                var binary = e.Payload;

//                var snapshot = serializer.FromBinary(binary, type);


//                return new SelectedSnapshot(metadata, snapshot);
//            }

//            protected override Task<SelectedSnapshot> LoadAsync(string persistenceId, SnapshotSelectionCriteria criteria)
//            {

//                var proxy = ActorProxy.Create<IServiceFabricSnaphotStore>(new ActorId(persistenceId), _servicefabricServiceUri);


//                var snapshots = proxy.selectSnapshotAsync(criteria.MaxSequenceNr, criteria.MaxTimeStamp);

//                var tokenSource = GetCancellationTokenSource();

                
//                return sqlCommand
//                    .ExecuteReaderAsync(tokenSource.Token)
//                    .ContinueWith(task =>
//                    {
//                        var reader = task.Result;
//                        try
//                        {
//                            return reader.Read() ? QueryMapper.Map(reader) : null;
//                        }
//                        finally
//                        {
//                            PendingOperations.Remove(tokenSource);
//                            reader.Close();
//                        }
//                    }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
//            }

//            protected override Task SaveAsync(SnapshotMetadata metadata, object snapshot)
//            {
//                var entry = ToSnapshotEntry(metadata, snapshot);
//                var sqlCommand = QueryBuilder.InsertSnapshot(entry);
//                CompleteCommand(sqlCommand);

//                var tokenSource = GetCancellationTokenSource();

//                return sqlCommand.ExecuteNonQueryAsync(tokenSource.Token)
//                    .ContinueWith(task =>
//                    {
//                        PendingOperations.Remove(tokenSource);
//                    }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.AttachedToParent);
//            }

//            protected override void Saved(SnapshotMetadata metadata) { }

//            protected override void Delete(SnapshotMetadata metadata)
//            {
//                var sqlCommand = QueryBuilder.DeleteOne(metadata.PersistenceId, metadata.SequenceNr, metadata.Timestamp);
//                CompleteCommand(sqlCommand);

//                sqlCommand.ExecuteNonQuery();
//            }

//            protected override void Delete(string persistenceId, SnapshotSelectionCriteria criteria)
//            {
//                var sqlCommand = QueryBuilder.DeleteMany(persistenceId, criteria.MaxSequenceNr, criteria.MaxTimeStamp);
//                CompleteCommand(sqlCommand);

//                sqlCommand.ExecuteNonQuery();
//            }

//            private void CompleteCommand(SqlCommand command)
//            {
//                command.Connection = _connection;
//                command.CommandTimeout = (int)_extension.SnapshotStoreSettings.ConnectionTimeout.TotalMilliseconds;
//            }

//            private CancellationTokenSource GetCancellationTokenSource()
//            {
//                var source = new CancellationTokenSource();
//                PendingOperations.AddLast(source);
//                return source;
//            }

//            private SnapshotEntry ToSnapshotEntry(SnapshotMetadata metadata, object snapshot)
//            {
//                var snapshotType = snapshot.GetType();
//                var serializer = Context.System.Serialization.FindSerializerForType(snapshotType);

//                var binary = serializer.ToBinary(snapshot);

//                return new SnapshotEntry(
//                    persistenceId: metadata.PersistenceId,
//                    sequenceNr: metadata.SequenceNr,
//                    timestamp: metadata.Timestamp,
//                    snapshotType: snapshotType.QualifiedTypeName(),
//                    snapshot: binary);
//            }
//        }
//    }
