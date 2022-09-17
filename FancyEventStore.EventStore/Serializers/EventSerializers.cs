using FancyEventStore.EventStore.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyEventStore.EventStore.Serializers
{
    //TODO Make Lazy initialization
    public static class EventSerializers
    {
        public static IEventSerializer Json = new JsonEventSerializer();
        public static IEventSerializer MessagePack = new MessagePackEventSerializer();
        public static IEventSerializer Protobuf = new ProtobufEventSerializer();    
    }
}
