using System.Text.Json;
using System.Text.Json.Serialization;

namespace FancyEventStore.Domain.Abstract
{
    public abstract class Aggregate : IAggregate
    {
        public Guid Id { get; protected set; } = default!;

        public long Version { get; protected set; }

        [NonSerialized] 
        private readonly List<object> _uncommittedEvents = new();

        public IEnumerable<object> DequeueUncommittedEvents()
        {
            var dequeuedEvents = _uncommittedEvents.ToArray();

            _uncommittedEvents.Clear();

            return dequeuedEvents;
        }

        protected void Enqueue(object @event)
        {
            _uncommittedEvents.Add(@event);
            Version++;
        }

        public abstract void When(object @event);
    }
}
