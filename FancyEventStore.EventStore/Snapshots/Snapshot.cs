using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore.Snapshots
{
    public class Snapshot
    {
        public int ShapshotId { get; set; }
        public Guid StreamId { get; set; }
        public string Data { get; set; }
        public long Version { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public EventStream EventStream { get; set; }
    }
}
