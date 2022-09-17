using FancyEventStore.EventStore.Snapshots;

namespace FancyEventStore.EventStore.Abstractions
{
    public interface ISnapshotPredicate
    {
        bool ShouldTakeSnapshot(SnapshotContext context);
    }
}
