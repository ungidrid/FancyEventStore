﻿using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test2
{
    public abstract class Test2Base: Tests.Test2Dapper
    {
        protected readonly IEventStore eventStore;
        protected readonly string resultFileName;
        protected readonly int retriesCount;
        protected Random temperatureProvider = new(1);
        protected Dictionary<int, List<long>> testCases;
        public Test2Base(IEventStore eventStore, string resultFileName, int retriesCount)
        {
            testCases = new()
            {
                {1, new List<long>() },
                {5, new List<long>() },
                {10, new List<long>() },
                {20, new List<long>() },
                {50, new List<long>() },
                {100, new List<long>() },
                {200, new List<long>() },
                {300, new List<long>() },
                {400, new List<long>() },
                {500, new List<long>() },
                {600, new List<long>() },
                {700, new List<long>() },
                {800, new List<long>() },
                {900, new List<long>() },
                {999, new List<long>() }
            };
            this.eventStore = eventStore;
            this.resultFileName = resultFileName;
            this.retriesCount = retriesCount;
        }

        public override async Task Run()
        {
            await CleanData();
            await FillData();

            foreach (var @case in testCases)
            {                 
                var measurementId = Guid.NewGuid();
                var measurement = TemperatureMeasurement.Start(measurementId);
                for (int j = 0; j < @case.Key; j++)
                {
                    measurement.Record(temperatureProvider.Next(-10, 40));
                }

                await eventStore.Store(measurement);

                for (int i = 0; i < retriesCount; i++)
                {
                    Console.WriteLine($"Case {@case.Key}; Attempt: {i}");


                    var sw = Stopwatch.StartNew();
                    var loaded = await eventStore.Rehydrate<TemperatureMeasurement>(measurementId);
                    sw.Stop();

                    testCases[@case.Key].Add(sw.ElapsedMilliseconds);
                }
            }

            File.Delete(resultFileName);
            foreach (var result in testCases)
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