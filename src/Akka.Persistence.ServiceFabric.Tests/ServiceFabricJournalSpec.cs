//-----------------------------------------------------------------------
// <copyright file="ServiceFabricJournalSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Configuration;
using Akka.Persistence.TestKit.Journal;

namespace Akka.Persistence.ServiceFabric.Tests
{
    public class ServiceFabricJournalSpec : JournalSpec
    {
        private static readonly Config SpecConfig = ConfigurationFactory.ParseString(@"
        akka.persistence {
            publish-plugin-commands = on
            journal {
                plugin = ""akka.persistence.journal.service-fabric""
                service-fabric {
                    class = ""Akka.Persistence.ServiceFabric.Journal.ServiceFabricJournal, Akka.Persistence.ServiceFabric""
                    plugin-dispatcher = ""akka.actor.default-dispatcher""
                    service-fabric-service-uri = ""fabric:/ServiceFabricEventJournalApplication""
                    //table-name = EventJournal
                    //schema-name = dbo
                    //auto-initialize = on
                    //connection-string = ""Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\Resources\\AkkaPersistenceSqlServerSpecDb.mdf;Integrated Security=True""
    }
            }
        }");

        public ServiceFabricJournalSpec()
            : base(SpecConfig, "ServiceFabricJournalSpec")
        {
         
            //HACK: we need to clear the journal for the active Pid.. this will do for now..   
            ServiceFabric.Journal.ServiceFabricJournal.ClearJournal(Pid, "fabric:/ServiceFabricEventJournalApplication");
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //DbCleanup.Clean();

        }
    }
}
