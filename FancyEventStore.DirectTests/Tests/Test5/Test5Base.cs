
using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore;
using FancyEventStore.EventStore.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace FancyEventStore.DirectTests.Tests.Test5
{
    public abstract class Test5Base: TestBase
    {
        protected readonly IEventStore eventStore;
        protected readonly IServiceProvider serviceProvider;
        protected readonly string resultFileName;
        protected readonly int retriesCount;
        protected Random temperatureProvider = new(1);
        protected Random delayProvider = new(1);
        protected int actionsCount = 500;
        protected int recordEachAction = 25;
        protected int threadsCount = 5;
        public Test5Base(IServiceProvider serviceProvider, string resultFileName, int retriesCount)
        {
            this.serviceProvider = serviceProvider;
            this.resultFileName = resultFileName;
            this.retriesCount = retriesCount;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            Guid measurementId;
            using (var scope = serviceProvider.CreateAsyncScope())
            {
                var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();
                measurementId = Guid.NewGuid();
                var measurement = TemperatureMeasurement.Start(measurementId);
                await eventStore.Store(measurement);
            }

            var actions = new ConcurrentDictionary<(int Task, int Action), (long TotalTime, int ErrorsCount)>();

            var tasks = new List<Thread>();
            for (var t = 0; t < threadsCount; t++)
            {
                int taskNumber = t;
                var thread = new Thread(() =>
                {
                    using var scope = serviceProvider.CreateScope();
                    var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

                    int i = 1;
                    Console.WriteLine($"Task {taskNumber} started");

                    while (i <= actionsCount / threadsCount)
                    {
                        Console.WriteLine($"Step {i}; Task {taskNumber}");
                        int errorsCount = 0;
                        var sw = Stopwatch.StartNew();
                        X:
                        try
                        {
                            var measurement = eventStore.Rehydrate<TemperatureMeasurement>(measurementId).Result;
                            measurement.Record(taskNumber);
                            eventStore.Store(measurement).Wait();
                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine($"Error {taskNumber}");
                            Thread.Sleep(delayProvider.Next(0, 200));
                            errorsCount++;
                            goto X;
                        }

                        sw.Stop();

                        actions.TryAdd((taskNumber, i), (sw.ElapsedMilliseconds, errorsCount));

                        i++;
                    }
                });

                tasks.Add(thread);
            }

            tasks.ForEach(x => x.Start());
            tasks.ForEach(x => x.Join());
           

            //File.Delete(resultFileName);
            //foreach (var result in actions)
            //{
            //    File.AppendAllText(resultFileName, $"({result.Key}, {result.Value.Average().ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}), ");
            //}
        }

        protected override async Task FillData()
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var eventStore = scope.ServiceProvider.GetRequiredService<IEventStore>();

            const int initialStreamsCount = 0;
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
