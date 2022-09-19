
using FancyEventStore.Common;
using FancyEventStore.Domain.Abstract;
using FancyEventStore.EventStore.Abstractions;
using FancyEventStore.EventStore.Snapshots;

namespace FancyEventStore.EventStore
{
    internal class UnsafeEventStore : IEventStore
    {
        private readonly IStore _store;
        private readonly IEnumerable<IProjection> _projections;
        private readonly EventStoreOptions _eventStoreOptions;

        public UnsafeEventStore(IStore store, IEnumerable<IProjection> projections, EventStoreOptions eventStoreOptions)
        {
            _store = store;
            _projections = projections;
            _eventStoreOptions = eventStoreOptions;
        }

        public async Task<TAggregate> Rehydrate<TAggregate>(Guid streamId, long? version = null) where TAggregate : IAggregate
        {
            var stream = await _store.GetStreamAsync(streamId);
            if (stream == null) return default;

            var latestSnapshot = await _store.GetNearestSnapshotAsync(streamId, version);
            var aggregate = latestSnapshot == null
                ? Activator.CreateInstance<TAggregate>()
                : (TAggregate)_eventStoreOptions.EventSerializer.Deserialize(latestSnapshot.Data, typeof(TAggregate));

            var events = await _store.GetEventsAsync(streamId, latestSnapshot?.Version + 1 ?? 0, version);

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

        public async Task Store<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate
        {
            var events = aggregate.DequeueUncommittedEvents();
            var initialVersion = aggregate.Version - events.Count();

            var currentEventVersion = initialVersion;

            var eventsToStore = events.Select(x => new Event
            {
                StreamId = aggregate.Id,
                Type = x.GetTypeName(),
                Data = _eventStoreOptions.EventSerializer.Serialize(x),
                Version = ++currentEventVersion
            }).ToList();

            await _store.AppendEventsAsync(eventsToStore);

            HandleProjections(events);
            await HandleShnapshots(aggregate, eventsToStore);
        }

        private async Task HandleShnapshots<TAggregate>(TAggregate aggregate, List<Event> eventsToStore) where TAggregate : IAggregate
        {
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
