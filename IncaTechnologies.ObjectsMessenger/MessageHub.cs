using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace IncaTecnologies.ObjectsMessenger
{

    /// <summary>
    /// Singleton object to handle <see cref="Messenger"/> events.
    /// </summary>
    public sealed class MessageHub
    {
        /// <summary>
        /// The default message hub manage the events related to the messengers.
        /// Set this object it should never be needed out side testing.
        /// </summary>
        public static MessageHub Default { get; } = new MessageHub();

        /// <summary>
        /// Exposes a way to intercept every action done by the messengers.
        /// </summary>
        public event EventHandler<(Messenger, MessengerEvent)>? MessengersEvents;

        internal void OnMessengerEvent(Messenger messenger, MessengerEvent messengerEvent) => MessengersEvents?.Invoke(this, (messenger, messengerEvent));

        private MessageHub() { }
    }

}
