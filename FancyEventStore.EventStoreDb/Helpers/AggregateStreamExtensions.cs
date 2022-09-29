using EventStore.Client;
using FancyEventStore.Domain.Abstract;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStoreDb.Helpers
{
    internal static class AggregateStreamExtensions
    {
        public static async Task<T?> AggregateStream<T>(
            this EventStoreClient eventStore,
            Guid id,
            CancellationToken cancellationToken,
            ulong? fromVersion = null
        ) where T : IAggregate
        {
            var readResult = eventStore.ReadStreamAsync(
                Direction.Forwards,
                StreamNameMapper.ToStreamId<T>(id),
                fromVersion ?? StreamPosition.Start,
                cancellationToken: cancellationToken
            );

            var readState = await readResult.ReadState;

            if (readState == ReadState.StreamNotFound)
                return default;

            var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

            var events = await readResult.ToListAsync();
            foreach (var @event in events)
            {
                var eventData = @event.Deserialize();

                aggregate.When(eventData);
            }

            var aggregateVersion = events.Max(x => x.OriginalEventNumber);
            aggregate.SetVersion((long)aggregateVersion.ToUInt64() + 1);

            return aggregate;
        }
    }
}
