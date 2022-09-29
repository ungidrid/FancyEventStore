using FancyEventStore.Common;
using FancyEventStore.Domain.Abstract;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStore.Snapshots;
using System.Text.Json;

namespace FancyEventStore.EventStore
{
    internal class DefaultEventStore : IEventStore
    {
        private readonly IStore _store;
        private readonly IEnumerable<IProjection> _projections;
        private readonly EventStoreOptions _eventStoreOptions;

        public DefaultEventStore(IStore store, IEnumerable<IProjection> projections, EventStoreOptions eventStoreOptions)
        {
            _store = store;
            _projections = projections;
            _eventStoreOptions = eventStoreOptions;
        }

        public async Task<TAggregate> Rehydrate<TAggregate>(
            Guid aggregateId, 
            long? version = null) where TAggregate : IAggregate
        {
            var stream = await _store.GetStreamAsync(aggregateId);
            if (stream == null) return default;

            var latestSnapshot = _eventStoreOptions.SnapshotPredicate == null 
                ? null 
                : await _store.GetNearestSnapshotAsync(aggregateId, version);

            var aggregate = latestSnapshot == null
                ? Activator.CreateInstance<TAggregate>()
                : (TAggregate)_eventStoreOptions.EventSerializer.Deserialize(latestSnapshot.Data, typeof(TAggregate));
            
            var events = await _store.GetEventsAsync(aggregateId, latestSnapshot?.Version + 1 ?? null, version);

            events.ToList().ForEach(e =>
            {
                var domainEvent = _eventStoreOptions.EventSerializer.Deserialize(
                    e.Data, 
                    TypeHelper.GetType(e.Type)
                );

                aggregate.When(domainEvent);
            });

            var aggregateVersion = events.Any() ? events.Max(x => x.Version) : latestSnapshot.Version;
            aggregate.SetVersion(aggregateVersion);

            return aggregate;
        }

        public async Task Store<TAggregate>(
            TAggregate aggregate) where TAggregate : IAggregate
        {
            var events = aggregate.DequeueUncommittedEvents();
            var initialVersion = aggregate.Version - events.Count();

            var eventStream = await GetStream(aggregate, initialVersion);

            var currentEventVersion = initialVersion;

            var eventsToStore = events.Select(x => new Event
            {
                StreamId = aggregate.Id,
                Type = x.GetTypeName(),
                Data = _eventStoreOptions.EventSerializer.Serialize(x),
                Version = ++currentEventVersion,
                Stream = eventStream
            }).ToList();

            eventStream.Version = currentEventVersion;

            await _store.AppendEventsAsync(eventStream, eventsToStore);

            //HandleProjections(events);
            //await HandleShnapshots(aggregate, eventsToStore);
        }

        private async Task<EventStream> GetStream<TAggregate>(TAggregate aggregate, long initialVersion) where TAggregate : IAggregate
        {
            var eventStream = await _store.GetStreamAsync(aggregate.Id);

            if (eventStream != null && eventStream.Version != initialVersion) throw new EventStoreConcurrencyException("Outer");

            eventStream ??= new EventStream()
            {
                StreamId = aggregate.Id,
            };
            return eventStream;
        }

        //TODO Think about possible concurrency problems when making snapshots
        private async Task HandleShnapshots<TAggregate>(TAggregate aggregate, List<Event> eventsToStore) where TAggregate : IAggregate
        {
            if (_eventStoreOptions.SnapshotPredicate == null) return;

            var latestSnapshot = await _store.GetNearestSnapshotAsync(aggregate.Id);

            var snapshotContext = new SnapshotContext(aggregate, eventsToStore, latestSnapshot?.CreatedAt, latestSnapshot?.Version);

            if (_eventStoreOptions.SnapshotPredicate.ShouldTakeSnapshot(snapshotContext))
            {
                var snapshot = new Snapshot
                {
                    StreamId = aggregate.Id,
                    Version = aggregate.Version,
                    Data = _eventStoreOptions.EventSerializer.Serialize(aggregate)
                };

                await _store.SaveSnapshot(snapshot);
            }
        }

        private void HandleProjections(IEnumerable<object> events)
        {
            events.ToList().ForEach(x =>
            {
                _projections.Where(p => p.CanHandle(x))
                    .ToList()
                    .ForEach(async p => await p.WhenAsync(x));
            });
        }
    }
}