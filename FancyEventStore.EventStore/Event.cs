namespace FancyEventStore.EventStore
{
    public class Event
    {
        public int Id { get; set; }
        public Guid StreamId { get; set; }
        public EventStream Stream { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
        public string Data { get; set; }
        public long Version { get; set; }
    }
}
