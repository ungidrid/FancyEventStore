using FancyEventStore.EventStore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FancyEventStore.EfCoreStore
{
    internal class EfCoreStoreRegistrar : IStoreRegistrar
    {
        private readonly Action<DbContextOptionsBuilder> _dbContextOptionsFactory;

        public EfCoreStoreRegistrar(Action<DbContextOptionsBuilder> dbContextOptionsFactory)
        {
            _dbContextOptionsFactory = dbContextOptionsFactory;
        }
        public void RegisterStore(IServiceCollection services)
        {
            services.AddScoped<IStore, EfCoreStore>();
            services.AddDbContext<EfCoreStoreContext>(_dbContextOptionsFactory);
        }
    }
}
