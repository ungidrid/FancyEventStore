using Dapper;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public class Test1Dapper : Test1Base
    {
        private readonly IDbContext _context;

        public Test1Dapper(IEventStore eventStore, IDbContext context, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
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
