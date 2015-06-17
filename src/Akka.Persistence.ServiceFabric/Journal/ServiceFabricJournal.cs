//-----------------------------------------------------------------------
// <copyright file="ServiceFabricSnapshotStore.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) Microsoft Corporation. All rights reserved. 
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Akka.Persistence.Journal;
using Microsoft.ServiceFabric.Actors;
using ServiceFabricPersistence.Interfaces;

namespace Akka.Persistence.ServiceFabric.Journal
{

    /// <summary>
    /// Persistent journal actor using SQL Server as persistence layer. It processes write requests
    /// one by one in synchronous manner, while reading results asynchronously.
    /// </summary>
    public class ServiceFabricJournal : SyncWriteJournal
    {

        private readonly ServiceFabricPersistenceExtension _extension;
        private readonly string _servicefabricServiceUri;
        private readonly Akka.Serialization.Serialization _serialization;

        protected readonly LinkedList<CancellationTokenSource> PendingOperations;

        public ServiceFabricJournal()
        {
            _extension = ServiceFabricPersistence.Instance.Apply(Context.System);
            _serialization = Context.System.Serialization;

            var settings = _extension.JournalSettings;
            _servicefabricServiceUri = settings.servicefabricServiceUri;
            PendingOperations = new LinkedList<CancellationTokenSource>();
        }

        protected override void PreStart()
        {
            base.PreStart();

            //TODO
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

        private IPersistentRepresentation Map(JournalEntry e)
        {
            var persistenceId = e.PersistenceId;
            var sequenceNr = e.SequenceNr;
            var isDeleted = e.IsDeleted;

            var payloadType = e.PayloadType;
            var type = Type.GetType(payloadType, true);

            var serializer = _serialization.FindSerializerForType(type);
            var payload = serializer.FromBinary(e.Payload, type);

            return new Persistent(payload, sequenceNr, persistenceId, isDeleted);
        }

        private static JournalEntry ToJournalEntry(IPersistentRepresentation message)
        {
            var payloadType = message.Payload.GetType();
            var serializer = Context.System.Serialization.FindSerializerForType(payloadType);

            return new JournalEntry(message.PersistenceId, message.SequenceNr, message.IsDeleted,
                payloadType.QualifiedTypeName(), serializer.ToBinary(message.Payload));
        }


        /// <summary>
        /// Asynchronously replays all requested messages related to provided <paramref name="persistenceId"/>,
        /// using provided sequence ranges (inclusive) with <paramref name="max"/> number of messages replayed
        /// (counting from the beginning). Replay callback is invoked for each replayed message.
        /// </summary>
        /// <param name="persistenceId">Identifier of persistent messages stream to be replayed.</param>
        /// <param name="fromSequenceNr">Lower inclusive sequence number bound. Unbound by default.</param>
        /// <param name="toSequenceNr">Upper inclusive sequence number bound. Unbound by default.</param>
        /// <param name="max">Maximum number of messages to be replayed. Unbound by default.</param>
        /// <param name="replayCallback">Action invoked for each replayed message.</param>
        public override Task ReplayMessagesAsync(string persistenceId, long fromSequenceNr, long toSequenceNr, long max, Action<IPersistentRepresentation> replayCallback)
        {
            var proxy = ActorProxy.Create<IServiceFabricEventJournal>(new ActorId(persistenceId), _servicefabricServiceUri);

            return proxy.GetMessagesAsync(fromSequenceNr, toSequenceNr, max).ContinueWith((events) =>
            {
                var messages = events.Result;

                foreach (var m in messages)
                {
                    replayCallback(Map(m));
                }
            });
        }

        /// <summary>
        /// Asynchronously reads a highest sequence number of the event stream related with provided <paramref name="persistenceId"/>.
        /// </summary>
        public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
        {
            var proxy = ActorProxy.Create<IServiceFabricEventJournal>(new ActorId(persistenceId), _servicefabricServiceUri);

            return proxy.GetHighestSequenceNrAsync();
        }

        /// <summary>
        /// Synchronously writes all persistent <paramref name="messages"/> inside SQL Server database.
        /// 
        /// Specific table used for message persistence may be defined through configuration within 
        /// 'akka.persistence.journal.sql-server' scope with 'schema-name' and 'table-name' keys.
        /// </summary>
        public override void WriteMessages(IEnumerable<IPersistentRepresentation> messages)
        {
            foreach (var m in messages)
            {
                var proxy = ActorProxy.Create<IServiceFabricEventJournal>(new ActorId(m.PersistenceId), _servicefabricServiceUri);
                proxy.WriteMessageAsync(ToJournalEntry(m));
            }
        }

        /// <summary>
        /// Synchronously deletes all persisted messages identified by provided <paramref name="persistenceId"/>
        /// up to provided message sequence number (inclusive). If <paramref name="isPermanent"/> flag is cleared,
        /// messages will still reside inside database, but will be logically counted as deleted.
        /// </summary>
        public override void DeleteMessagesTo(string persistenceId, long toSequenceNr, bool isPermanent)
        {
            var proxy = ActorProxy.Create<IServiceFabricEventJournal>(new ActorId(persistenceId), _servicefabricServiceUri);
            proxy.DeleteMessagesToAsync(toSequenceNr, isPermanent);
        }


        private CancellationTokenSource GetCancellationTokenSource()
        {
            var source = new CancellationTokenSource();
            PendingOperations.AddLast(source);
            return source;
        }

        public static void ClearJournal(string persistenceId,string servicefabricServiceUri)
        {
            var proxy = ActorProxy.Create<IServiceFabricEventJournal>(new ActorId(persistenceId), servicefabricServiceUri);
            proxy.ClearAsync().Wait();
        }
    }
}
