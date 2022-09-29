using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public class Test5EventStoreDb : Test5Base
    {
        public Test5EventStoreDb(IServiceProvider serviceProvider, string resultFileName, int threadsCount) : base(serviceProvider, resultFileName, threadsCount)
        {
        }

        protected override Task CleanData()
        {
            return Task.CompletedTask;
        }
    }
}
