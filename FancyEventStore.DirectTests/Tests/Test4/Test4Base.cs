using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test4
{
    //Create aggregate and record some number of events
    //Can test with snapshots
    public abstract class Test4Base : TestBase
    {
        protected readonly IEventStore eventStore;
        protected readonly string resultFileName;
        protected readonly int retriesCount;
        protected Random temperatureProvider = new(1);
        protected int actionsCount = 5000;
        protected int recordEachAction = 5;
        public Test4Base(IEventStore eventStore, string resultFileName, int retriesCount)
        {
            this.eventStore = eventStore;
            this.resultFileName = resultFileName;
            this.retriesCount = retriesCount;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            var actions = new Dictionary<int, List<long>>();

            for (int j = 0; j < retriesCount; j++)
            {
                var measurementId = Guid.NewGuid();
                Console.WriteLine($"Try {j + 1}");
                for (int i = 1; i <= actionsCount; i++)
                {
                    Console.WriteLine($"Step {i}");
                    var sw = Stopwatch.StartNew();
                    TemperatureMeasurement measurement;
                    if (i == 1)
                    {
                        measurement = TemperatureMeasurement.Start(measurementId);
                    }
                    else
                    {
                        measurement = await eventStore.Rehydrate<TemperatureMeasurement>(measurementId);
                        measurement.Record(temperatureProvider.Next(-20, 10));
                    }

                    await eventStore.Store(measurement);
                    sw.Stop();

                    if (i == 1 || i % recordEachAction == 0)
                    {
                        if (!actions.TryGetValue(i, out var _))
                        {
                            actions.Add(i, new List<long>());
                        }

                        actions[i].Add(sw.ElapsedMilliseconds);
                    }
                }

            }


            File.Delete(resultFileName);
            foreach (var result in actions)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            }
        }

        protected override async Task FillData()
        {
            const int initialStreamsCount = 1000;
            var measurementsCountProvider = new Random(1);
            var temperatureProvider = new Random(1);

            for (int i = 0; i < initialStreamsCount; i++)
            {
                var temperatureMeasurement = TemperatureMeasurement.Start(Guid.NewGuid());

                var measurementsCount = measurementsCountProvider.Next(1, 20);
                for (int j = 0; j < measurementsCount; j++)
                {
                    temperatureMeasurement.Record(temperatureProvider.Next(-10, 40));
                }

                await eventStore.Store(temperatureMeasurement);
            }
        }
    }
}
