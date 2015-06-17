using Akka.Persistence.TestKit.Snapshot;
using Akka.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Persistence.ServiceFabric.Tests
{
    public class ServiceFabricSnapshotStoreSpec : SnapshotStoreSpec
    {
        private static readonly Config SpecConfig = ConfigurationFactory.ParseString(@"
        akka.persistence {
            publish-plugin-commands = on
        }");

        public ServiceFabricSnapshotStoreSpec()
            : base(SpecConfig, "ServiceFabricSnapshotStoreSpec")
        {
            //DbCleanup.Clean();
            //Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //DbCleanup.Clean();

        }
    }
}
