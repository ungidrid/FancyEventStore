using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStore.Serializers;
using FancyEventStore.EventStore.Snapshots;

namespace FancyEventStore.EventStore
{
    public class EventStoreOptions
    {
        public IStoreRegistrar StoreRegistrar { get; set; }
        public ISnapshotPredicate SnapshotPredicate { get; set; } = new EachNEventsSnapshotPredicate(10);
        public IEventSerializer EventSerializer { get; set; } = new JsonEventSerializer();
    }
}
