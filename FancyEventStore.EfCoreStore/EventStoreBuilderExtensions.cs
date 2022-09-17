using FancyEventStore.EventStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EfCoreStore
{
    public static class EventStoreBuilderExtensions
    {
        public static EventStoreOptions UseEfCore(this EventStoreOptions optionsBuilder, Action<DbContextOptionsBuilder> dbContextOptions)
        {
            optionsBuilder.StoreRegistrar = new EfCoreStoreRegistrar(dbContextOptions);
            return optionsBuilder;
        }
    }
}
