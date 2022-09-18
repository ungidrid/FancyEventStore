﻿using AntActor.Core;
using FancyEventStore.Domain.TemperatureMeasurement;
using FancyEventStore.EventStore.Abstractions;

namespace FancyEventStore.Api.Actors
{
    public interface ITemperatureMeasurementAction
    {
    }

    public class StartMeasurementAction: ReplyableMessageBase<bool>, ITemperatureMeasurementAction 
    {
        public StartMeasurementAction(ReplyChanel<bool> replyChannel): base(replyChannel)
        {
        }
    }

    public class RecordMeasurementAction : ReplyableMessageBase<bool>, ITemperatureMeasurementAction
    {
        public decimal Temperature { get; }

        public RecordMeasurementAction(decimal temperature, ReplyChanel<bool> replyChannel) : base(replyChannel)
        {
            Temperature = temperature;
        }
    }

    public class TemperatureMeasurementAnt : AbstractAnt<ITemperatureMeasurementAction>
    {
        private readonly Guid _id;
        private TemperatureMeasurement _measurement;

        private readonly IEventStore _eventStore;

        public TemperatureMeasurementAnt(IEventStore eventStore, string id)
        {
            _eventStore = eventStore;
            _id = Guid.Parse(id);
        }

        protected override async Task OnActivateAsync()
        {
            _measurement = await _eventStore.Rehydrate<TemperatureMeasurement>(_id);
        }

        public async Task StartMeasurement() => await PostAndReply<bool>(rc => new StartMeasurementAction(rc));
        public async Task Record(decimal temperature) => await PostAndReply<bool>(rc => new RecordMeasurementAction(temperature, rc));

        protected override async Task HandleMessage(ITemperatureMeasurementAction message)
        {
            switch (message)
            {
                case StartMeasurementAction a:
                    if (_measurement != null) throw new Exception("Measurement already started");
                    _measurement = TemperatureMeasurement.Start(_id);
                    break;
                case RecordMeasurementAction a:
                    if (_measurement == null) throw new Exception("Measurement not started");
                    _measurement.Record(a.Temperature);
                    break;
            }

            await _eventStore.Store(_measurement);
            (message as ReplyableMessageBase<bool>).ReplyChannel.Reply(true);
        }

        protected override Task<HandleResult> HandleError(AntMessage<ITemperatureMeasurementAction> message, Exception ex) => HandleResult.OkTask();
    }
}