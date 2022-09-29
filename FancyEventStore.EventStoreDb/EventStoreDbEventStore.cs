using EventStore.Client;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStoreDb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FancyEventStore.EventStoreDb
{
    internal class EventStoreDbEventStore : IEventStore
    {
        private readonly EventStoreClient _eventStoreClient;

        public EventStoreDbEventStore(EventStoreClient eventStoreClient)
        {
            _eventStoreClient = eventStoreClient;
        }

        public async Task<TAggregate> Rehydrate<TAggregate>(Guid streamId, long? version = null) where TAggregate : FancyEventStore.Domain.Abstract.IAggregate
        {
            return await _eventStoreClient.AggregateStream<TAggregate>(streamId, CancellationToken.None);
        }

        public async Task Store<TAggregate>(TAggregate aggregate) where TAggregate : FancyEventStore.Domain.Abstract.IAggregate
        {
            var events = aggregate.DequeueUncommittedEvents();
            var eventsToStore = events
                .Select(EventStoreDBSerializer.ToJsonEventData).ToArray();

            var expectedVersion = aggregate.Version - eventsToStore.Count();
            var optimisticConcurrencyParam = expectedVersion == 0 ? StreamRevision.None : (StreamRevision)(ulong)(expectedVersion - 1);

            await _eventStoreClient.AppendToStreamAsync(
                StreamNameMapper.ToStreamId<TAggregate>(aggregate.Id), 
                optimisticConcurrencyParam,
                eventsToStore
            );
        }
    }
}
