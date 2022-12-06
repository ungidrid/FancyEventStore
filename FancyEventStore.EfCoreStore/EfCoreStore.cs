using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Snapshots;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FancyEventStore.EfCoreStore
{
    internal class EfCoreStore : IStore
    {
        private readonly EfCoreStoreContext _context;

        public EfCoreStore(EfCoreStoreContext context)
        {
            _context = context;
        }

        public async Task AppendEventsAsync(EventStream stream, IEnumerable<Event> events)
        {
            try
            {
                await _context.AddRangeAsync(events);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException) 
            {
                throw new EventStoreConcurrencyException();
            }
        }

        public async Task<IEnumerable<Event>> GetEventsAsync(
            Guid streamId, 
            long? fromVersion = null,
            long? toVersion = null)
        {
            return await _context.Events
                .Where(x => 
                    x.StreamId == streamId 
                    && (fromVersion == null || x.Version >= fromVersion)
                    && (toVersion == null || x.Version <= toVersion)
                 )
                .ToListAsync();
        }

        public async Task<EventStream> GetStreamAsync(Guid streamId)
        {
            _context.ChangeTracker.Clear();
            return await _context.EventStreams.FirstOrDefaultAsync(x => x.StreamId == streamId);
        }

        public async Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null)
        {
            return await _context.Snapshots
                .Where(x =>
                    x.StreamId == streamId 
                    && (version == null || x.Version <= version))
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();
        }

        public async Task SaveSnapshot(Snapshot snapshot)
        {
            _context.Snapshots.Add(snapshot);
            await _context.SaveChangesAsync();
        }
    }
}
