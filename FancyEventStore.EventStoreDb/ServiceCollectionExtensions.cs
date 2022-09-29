using EventStore.Client;
using FancyEventStore.EventStore.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FancyEventStore.EventStoreDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStoreDB(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(new EventStoreClient(EventStoreClientSettings.Create(connectionString)));
            services.AddScoped<IEventStore, EventStoreDbEventStore>();
            return services;
        }
    }
}
