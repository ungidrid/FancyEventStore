
namespace FancyEventStore.EventStore.Abstractions
{
    public interface IProjection
    {
        bool CanHandle(object @event);
        Task WhenAsync(object @event);
    }
}
