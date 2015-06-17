using Akka.Persistence.TestKit.Snapshot;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
