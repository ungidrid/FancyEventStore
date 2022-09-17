using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//For DateTimeOffset serialization by protobuf
namespace FancyEventStore.Common
{
    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        [ProtoMember(1)]
        public string DateTimeString { get; set; }

        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        {
            return new DateTimeOffsetSurrogate { DateTimeString = value.ToString("u") };
        }

        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        {
            return DateTimeOffset.Parse(value.DateTimeString);
        }
    }
}
