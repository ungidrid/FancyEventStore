using FancyEventStore.DapperProductionStore;
using FancyEventStore.EfCoreStore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public class Test5Ef: Test5Base
    {
        private readonly EfCoreStoreContext _context;

        public Test5Ef(IServiceProvider serviceProvider, EfCoreStoreContext context, string resultFileName, int retriesCount) : base(serviceProvider, resultFileName, retriesCount)
        {
            _context = context;
        }

        protected override async Task CleanData()
        {
            var sql =
                @"DELETE FROM Events;
                  DELETE FROM EventStreams;";

            await _context.Database.ExecuteSqlRawAsync(sql);
            _context.ChangeTracker.Clear();
        }
    }
}
