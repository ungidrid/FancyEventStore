using Dapper;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test4
{
    public class Test4Dapper: Test4Base
    {
        private readonly IDbContext _context;

        public Test4Dapper(IEventStore eventStore, IDbContext context, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
