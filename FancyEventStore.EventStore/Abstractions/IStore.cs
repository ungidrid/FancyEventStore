using FancyEventStore.EventStore.Snapshots;

namespace FancyEventStore.EventStore
{
    public interface IStore
    {
        Task AppendEventsAsync(EventStream stream, IEnumerable<Event> events);
        Task<IEnumerable<Event>> GetEventsAsync(Guid streamId, long? fromVersion = null, long? toVersion = null);
        Task<Snapshot> GetNearestSnapshotAsync(Guid streamId, long? version = null);
        Task<EventStream> GetStreamAsync(Guid streamId);
        Task SaveSnapshot(Snapshot snapshot); 
    }
}
