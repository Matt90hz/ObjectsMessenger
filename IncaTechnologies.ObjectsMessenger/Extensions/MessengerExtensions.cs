using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace IncaTechnologies.ObjectsMessenger
{
    public static class MessengerExtensions
    {
        public static void SendProperty<TSender, TReceiver, TMessage>(this Messenger<TSender, TReceiver, TMessage> messenger, TSender sender, ref TMessage field, TMessage value)
        {
            field = value;
            messenger.Send(sender);
        }
    }
}
