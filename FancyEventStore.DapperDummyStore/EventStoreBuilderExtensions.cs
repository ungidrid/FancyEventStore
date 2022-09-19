using FancyEventStore.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DapperDummyStore
{
    public static class EventStoreBuilderExtensions
    {
        public static EventStoreOptions UseDummyDapperStore(this EventStoreOptions optionsBuilder, string connectionString)
        {
            optionsBuilder.StoreRegistrar = new DapperDummyStoreRegistrar(connectionString);
            return optionsBuilder;
        }
    }
}
