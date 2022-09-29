using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test4
{
    public class Test4EventStoreDb : Test4Base
    {
        public Test4EventStoreDb(IEventStore eventStore, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
        {
        }

        protected override Task CleanData()
        {
            return Task.CompletedTask;
        }
    }
}
