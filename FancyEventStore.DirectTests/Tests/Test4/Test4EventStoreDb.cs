using FancyEventStore.EventStore.Abstractions;

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
