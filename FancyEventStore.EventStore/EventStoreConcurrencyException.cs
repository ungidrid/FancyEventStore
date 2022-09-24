namespace FancyEventStore.EventStore
{
    public class EventStoreConcurrencyException : Exception
    {
        public EventStoreConcurrencyException(string message = "") : base(message)
        {
        }
    }
}
