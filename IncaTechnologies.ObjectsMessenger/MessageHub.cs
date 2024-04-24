using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;

namespace IncaTechnologies.ObjectsMessenger
{

    /// <summary>
    /// Singleton object to handle <see cref="Messenger"/> events.
    /// </summary>
    public sealed class MessageHub
    {
        readonly Subject<(Messenger, MessengerEvent)> _events = new Subject<(Messenger, MessengerEvent)>();
        readonly Subject<(Messenger, Exception)> _errors = new Subject<(Messenger, Exception)>();

        /// <summary>
        /// The default message hub manage the events related to the messengers.
        /// Set this object it should never be needed out side testing.
        /// </summary>
        public static MessageHub Default { get; } = new MessageHub();

        /// <summary>
        /// Sequence of events raised by all the registered <see cref="Messenger"/>.
        /// </summary>
        public IObservable<(Messenger, MessengerEvent)> MessengersEvents => _events;

        /// <summary>
        /// Sequence of error thrown by all the registered <see cref="Messenger"/>.
        /// </summary>
        public IObservable<(Messenger, Exception)> MessengersErrors => _errors;

        private MessageHub() { }

        /// <summary>
        /// Register the <paramref name="messenger"/> in the hub.
        /// </summary>
        /// <param name="messenger">The <see cref="Messenger"/> to be registered.</param>
        /// <returns>It self.</returns>
        public MessageHub RegisterMessenger(Messenger messenger)
        {
            messenger.Events.Subscribe(@event => _events.OnNext((messenger, @event)));
            messenger.Errors.Subscribe(exception => _errors.OnNext((messenger, exception)));

            return this;
        }
    }

}
