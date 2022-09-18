using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntActor.Core
{
    public class ReplyableMessageBase<T>
    {
        public ReplyChanel<T> ReplyChannel { get; }
        public ReplyableMessageBase(ReplyChanel<T> replyChannel)
        {
            ReplyChannel = replyChannel;
        }
    }
}
