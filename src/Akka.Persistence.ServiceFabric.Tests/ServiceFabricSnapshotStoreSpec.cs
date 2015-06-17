//-----------------------------------------------------------------------
// <copyright file="ServiceFabricSnapshotStoreSpec.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2015 Typesafe Inc. <http://www.typesafe.com>
//     Copyright (C) 2013-2015 Akka.NET project <https://github.com/akkadotnet/akka.net>
//     Copyright (C) Nethouse Örebro AB <http://akka.nethouse.se>
// </copyright>
//-----------------------------------------------------------------------

using Akka.Persistence.TestKit.Snapshot;
using Akka.Configuration;

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
