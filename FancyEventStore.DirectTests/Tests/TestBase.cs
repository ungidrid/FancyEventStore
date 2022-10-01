namespace FancyEventStore.DirectTests.Tests
{
    public abstract class TestBase
    {
        public abstract Task Run();
        protected abstract Task CleanData();
        protected abstract Task FillData();
    }
}
