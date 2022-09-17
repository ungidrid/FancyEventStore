using FancyEventStore.EventStore.Abstractions;

namespace FancyEventStore.EventStore.Snapshots
{
    public class EachNEventsSnapshotPredicate : ISnapshotPredicate
    {
        private readonly int _n;

        public EachNEventsSnapshotPredicate(int n)
        {
            _n = n;
        }

        public bool ShouldTakeSnapshot(SnapshotContext context)
        {
            return context.Aggregate.Version % _n == 0;
        }
    }
}
