using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EfCoreStore;
using FancyEventStore.EventStore.Abstractions;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public abstract class Test1Base : Test2Dapper
    {
        protected readonly IEventStore EventStore;
        protected readonly string resultFileName;
        protected readonly int retriesCount;
        protected Random temperatureProvider = new(1);
        protected Dictionary<int, List<long>> testCases;

        public Test1Base(IEventStore eventStore, string resultFileName, int retriesCount)
        {
            EventStore = eventStore;

            this.resultFileName = resultFileName;
            this.retriesCount = retriesCount;
            testCases = new()
            {
                {1, new List<long>() },
                {2, new List<long>() },
                {3, new List<long>() },
                {4, new List<long>() },
                {5, new List<long>() },
                {6, new List<long>() },
                {7, new List<long>() },
                {8, new List<long>() },
                {9, new List<long>() },
                {10, new List<long>() },
                {15, new List<long>() },
                {20, new List<long>() },
                {25, new List<long>() },
                {30, new List<long>() },
                {35, new List<long>() },
                {40, new List<long>() },
                {45, new List<long>() },
                {50, new List<long>() },
                {55, new List<long>() },
                {60, new List<long>() },
                {65, new List<long>() },
                {70, new List<long>() },
                {75, new List<long>() },
                {80, new List<long>() },
                {85, new List<long>() },
                {90, new List<long>() },
                {95, new List<long>() },
                {100, new List<long>() },
            };
        }

        public override async Task Run()
        {
            foreach (var @case in testCases)
            {
                for (int i = 0; i < retriesCount; i++)
                {
                    await CleanData();
                    await FillData();

                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");

                    var measurement = TemperatureMeasurement.Start(Guid.NewGuid());

                    for (int j = 0; j < @case.Key; j++)
                    {
                        measurement.Record(temperatureProvider.Next(-10, 40));
                    }

                    var sw = Stopwatch.StartNew();
                    await EventStore.Store(measurement);
                    sw.Stop();

                    testCases[@case.Key].Add(sw.ElapsedMilliseconds);
                }
            }

            File.Delete(resultFileName);
            foreach(var result in testCases)
            {
                File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            }
        }

        protected override async Task FillData()
        {
            const int initialStreamsCount = 100;
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

                await EventStore.Store(temperatureMeasurement);
            }
        }
    }
}
