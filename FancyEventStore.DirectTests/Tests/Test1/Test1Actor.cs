using AntActor.Core;
using FancyEventStore.Api.Actors;
using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.DirectTests.Tests.Test1
{
    public class Test1Actor : Test1Base
    {
        private readonly Anthill _anthill;

        public Test1Actor(IEventStore eventStore, Anthill anthill, string resultFileName, int retriesCount) : base(eventStore, resultFileName, retriesCount)
        {
            this._anthill = anthill;
        }



        protected override Task CleanData()
        {
            throw new NotImplementedException();
        }
    }
}
