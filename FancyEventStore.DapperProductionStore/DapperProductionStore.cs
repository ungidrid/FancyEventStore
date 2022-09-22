using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;

namespace FancyEventStore.DapperProductionStore
{
    public class DapperProductionStore : IStore
    {
        private readonly IDbContext _context;

        public DapperProductionStore(IDbContext context)
        {
            _context = context;
        }

        public Task AppendEventsAsync(IEnumerable<Event> events)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetEventsAsync(Guid streamId, long? fromVersion = null, long? toVersion = null)
        {
            throw new NotImplementedException();
        }

        public Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            throw new NotImplementedException();
        }

        public Task<EventStream> GetStreamAsync(Guid streamId)
        {
            throw new NotImplementedException();
        }

        public Task SaveSnapshot(Snapshot snapshot)
        {
            throw new NotImplementedException();
        }
    }
}
