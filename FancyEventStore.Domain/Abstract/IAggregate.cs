namespace FancyEventStore.Domain.Abstract
{
    public interface IAggregate
    {
        Guid Id { get; }
        long Version { get; }
        IEnumerable<object> DequeueUncommittedEvents();
        void When(object @event);
    }
}
