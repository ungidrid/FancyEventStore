using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests
{
    public abstract class TestBase
    {
        public abstract Task Run();
        protected abstract Task CleanData();
        protected abstract Task FillData();
    }
}
