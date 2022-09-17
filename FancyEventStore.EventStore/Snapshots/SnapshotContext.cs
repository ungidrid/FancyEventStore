using FancyEventStore.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore.Snapshots
{
    public class SnapshotContext
    {
        public IAggregate Aggregate { get; }
        public IEnumerable<object> CommitedEvents { get; }
        public DateTime? LastSnapshotCreatedAt { get; }
        public long? LastSnapshotVersion { get; }

        public SnapshotContext(IAggregate aggregate, IEnumerable<object> commitedEvents, DateTime? lastSnapshotCreatedAt, long? lastSnapshotVersion)
        {
            Aggregate = aggregate;
            CommitedEvents = commitedEvents;
            LastSnapshotCreatedAt = lastSnapshotCreatedAt;
            LastSnapshotVersion = lastSnapshotVersion;
        }
    }
}
