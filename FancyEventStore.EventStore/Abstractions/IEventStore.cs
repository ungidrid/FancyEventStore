using FancyEventStore.Domain.Abstract;

namespace FancyEventStore.EventStore.Abstractions
{
    public interface IEventStore
    {
        Task<TAggregate> Rehydrate<TAggregate>(Guid streamId, long? version = null) where TAggregate : IAggregate;
        Task Store<TAggregate>(TAggregate aggregate) where TAggregate : IAggregate;
    }
}
