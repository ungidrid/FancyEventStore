using Dapper;
using FancyEventStore.DapperProductionStore;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public class Test5Dapper : Test5Base
    {
        private readonly IDbContext _context;

        public Test5Dapper(IServiceProvider serviceProvider, IDbContext context, string resultFileName, int retriesCount) : base(serviceProvider, resultFileName, retriesCount)
        {
            _context = context;
        }

        protected override async Task CleanData()
        {
            var sql =
                @"DELETE FROM Events;
                  DELETE FROM EventStreams;";

            await _context.Connection.ExecuteAsync(sql);
        }
    }
}
