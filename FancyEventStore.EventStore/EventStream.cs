using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore
{
    public class EventStream
    {
        public Guid StreamId { get; set; }
        public long Version { get; set; }
        public byte[] Timestamp { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
