using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.MongoDbStore.Entities
{
    internal class FilteredEventsCollection
    {
        [BsonId]
        public Guid StreamId { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
