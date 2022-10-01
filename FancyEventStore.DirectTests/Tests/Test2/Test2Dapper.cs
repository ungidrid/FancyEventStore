using Dapper;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;

namespace FancyEventStore.DirectTests.Tests.Test2
{
    public class Test2Dapper : Test2Base
    {
        private readonly IDbContext _context;

        public Test2Dapper(IEventStore eventStore, IDbContext context, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
