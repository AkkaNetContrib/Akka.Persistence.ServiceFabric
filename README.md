## Akka.Persistence.ServiceFabric

Akka Persistence journal and snapshot store backed by Azure Service Fabric.

**WARNING: Akka.Persistence.ServiceFabric plugin is still in beta and it's mechanics described bellow may be still subject to change**.

##Design
Service Fabric plug-in is implemented using a standalone Service Fabric that persist messages in a fully replicated way so it guarantee availability and failover. The Service Fabric service is implemented using Reliable Actors: the persistence storage is modelled via Stateful actors.

## Project Structure
The plug-in is composed of two Visual Studio projects:

    * Akka.Persistence.ServiceFabric: it is the implementation of the Akka Persistence interface(s)
    * ServiceFabricEventJournal: this is the project that contains the implementation of the Service Fabric service. Service Fabric tools for Visual Studio are supported on VS2015 only so it needs to be a separate project

## Status
This is very preliminary version V0.2 of the plug-in and used mainly to test the intial design based on a Reliable Actors service. Only the Journal interface has been implemented and basic testing has been done with the PersistenceSample app.
