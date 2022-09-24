using Dapper;
using FancyEventStore.DapperProductionStore;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
