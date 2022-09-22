using FancyEventStore.EventStore;

namespace FancyEventStore.DapperProductionStore
{
    public static class EventStoreBuilderExtensions
    {
        public static EventStoreOptions UseDapperStore(this EventStoreOptions optionsBuilder, string connectionString)
        {
            optionsBuilder.StoreRegistrar = new DapperProductionStoreRegistrar(connectionString);
            return optionsBuilder;
        }
    }
}
