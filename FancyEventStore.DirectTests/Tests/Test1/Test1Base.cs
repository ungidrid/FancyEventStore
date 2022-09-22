using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EfCoreStore;
using FancyEventStore.EventStore.Abstractions;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public abstract class Test1Base : TestBase
    {
        protected readonly IEventStore EventStore;
        private readonly string _resultFileName;

        private Random _temperatureProvider = new(1);
        private Dictionary<int, List<long>> _testCases;

        public Test1Base(IEventStore eventStore, string resultFileName)
        {
            EventStore = eventStore;

            _resultFileName = resultFileName;
            _testCases = new()
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
            await CleanData();
            await FillData();

            foreach (var @case in _testCases)
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");

                    var measurement = TemperatureMeasurement.Start(Guid.NewGuid());

                    for (int j = 0; j < @case.Key; j++)
                    {
                        measurement.Record(_temperatureProvider.Next(-10, 40));
                    }

                    var sw = Stopwatch.StartNew();
                    await EventStore.Store(measurement);
                    sw.Stop();

                    _testCases[@case.Key].Add(sw.ElapsedMilliseconds);
                }
            }

            foreach(var result in _testCases)
            {
                File.AppendAllText(_resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
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

                var measurementsCount = measurementsCountProvider.Next(1, 200);
                for (int j = 0; j < measurementsCount; j++)
                {
                    temperatureMeasurement.Record(temperatureProvider.Next(-10, 40));
                }

                await EventStore.Store(temperatureMeasurement);
            }
        }
    }
}
