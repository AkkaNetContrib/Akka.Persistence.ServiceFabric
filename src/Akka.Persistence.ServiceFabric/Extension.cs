using System;
using Akka.Actor;
using Akka.Configuration;

namespace Akka.Persistence.ServiceFabric 
{
    /// <summary>
    /// Configuration settings representation targeting Azure Service Fabric journal actor.
    /// </summary>
    public class JournalSettings
    {
        public const string ConfigPath = "akka.persistence.journal.service-fabric";

        /// <summary>
        /// Flag determining in in case of event journal table missing, it should be automatically initialized.
        /// </summary>
        public bool AutoInitialize { get; private set; }

        public String servicefabricServiceUri { get; private set; }

        public JournalSettings(Config config)
        {
            if (config == null) throw new ArgumentNullException("config", "ServiceFabric journal settings cannot be initialized, because required HOCON section couldn't been found");

            servicefabricServiceUri = config.GetString("service-fabric-service-uri");
  
        }
    }

    /// <summary>
    /// Configuration settings representation targeting Sql Server snapshot store actor.
    /// </summary>
    public class SnapshotStoreSettings
    {
        public const string ConfigPath = "akka.persistence.snapshot-store.service-fabric";

        public String servicefabricServiceUri { get; private set; }

        /// <summary>
        /// Flag determining in in case of snapshot store table missing, it should be automatically initialized.
        /// </summary>
        public bool AutoInitialize { get; private set; }



        public SnapshotStoreSettings(Config config)
        {
            if (config == null) throw new ArgumentNullException("config", "ServicFabric snapshot store settings cannot be initialized, because required HOCON section couldn't been found");

            servicefabricServiceUri = config.GetString("service-fabric-service-uri");

        }
    }

    /// <summary>
    /// An actor system extension initializing support for SQL Server persistence layer.
    /// </summary>
    public class ServiceFabricPersistenceExtension : IExtension
    {
        /// <summary>
        /// Journal-related settings loaded from HOCON configuration.
        /// </summary>
        public readonly JournalSettings JournalSettings;

        /// <summary>
        /// Snapshot store related settings loaded from HOCON configuration.
        /// </summary>
        public readonly SnapshotStoreSettings SnapshotStoreSettings;

        public ServiceFabricPersistenceExtension(ExtendedActorSystem system)
        {
            system.Settings.InjectTopLevelFallback(ServiceFabricPersistence.DefaultConfiguration());

            JournalSettings = new JournalSettings(system.Settings.Config.GetConfig(JournalSettings.ConfigPath));
            SnapshotStoreSettings = new SnapshotStoreSettings(system.Settings.Config.GetConfig(SnapshotStoreSettings.ConfigPath));

            if (JournalSettings.AutoInitialize)
            {
                // TODO    
            }

            if (SnapshotStoreSettings.AutoInitialize)
            {
                //TODO    
            }
        }
    }

    /// <summary>
    /// Singleton class used to setup SQL Server backend for akka persistence plugin.
    /// </summary>
    public class ServiceFabricPersistence : ExtensionIdProvider<ServiceFabricPersistenceExtension>
    {
        public static readonly ServiceFabricPersistence Instance = new ServiceFabricPersistence();

        /// <summary>
        /// Initializes a SQL Server persistence plugin inside provided <paramref name="actorSystem"/>.
        /// </summary>
        public static void Init(ActorSystem actorSystem)
        {
            Instance.Apply(actorSystem);
        }

        private ServiceFabricPersistence() { }
        
        /// <summary>
        /// Creates an actor system extension for akka persistence SQL Server support.
        /// </summary>
        /// <param name="system"></param>
        /// <returns></returns>
        public override ServiceFabricPersistenceExtension CreateExtension(ExtendedActorSystem system)
        {
            return new ServiceFabricPersistenceExtension(system);
        }

        /// <summary>
        /// Returns a default configuration for akka persistence SQL Server-based journals and snapshot stores.
        /// </summary>
        /// <returns></returns>
        public static Config DefaultConfiguration()
        {
            return ConfigurationFactory.FromResource<ServiceFabricPersistence>("Akka.Persistence.ServiceFabric.service-fabric.conf");
        }
    }
}