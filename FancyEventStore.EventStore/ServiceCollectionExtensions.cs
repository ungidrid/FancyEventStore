using FancyEventStore.EventStore.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FancyEventStore.EventStore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services, Assembly projectionsAssembly, Action<EventStoreOptions> eventStoreOptionsFactory, bool useUnsafeStore = false)
        {
            if (useUnsafeStore)
            {
                services.AddScoped<IEventStore, UnsafeEventStore>();
            }
            else
            {
                services.AddScoped<IEventStore, DefaultEventStore>();
            }

            services.Scan(scan =>
                scan.FromAssemblies(projectionsAssembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IProjection)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());

            var eventStoreOptions = new EventStoreOptions();
            eventStoreOptionsFactory(eventStoreOptions);

            eventStoreOptions.StoreRegistrar.RegisterStore(services);

            services.AddSingleton(eventStoreOptions);

            return services;
        }
    }
}
