using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using IncaTecnologies.ObjectsMessenger;
using IncaTechnologies.ObjectsMessenger;

namespace IncaTechnologies.ObjectsMessenger
{
    /// <summary>
    /// Marker for <see cref="Messenger{TSender, TMessage}"/> and <see cref="Messenger{TSender, TReceiver, TMessage}"/> objects.
    /// </summary>
    public abstract class Messenger { }

    /// <summary>
    /// Implement this object to allow the transmission of <typeparamref name="TMessage"/> from <typeparamref name="TSender"/> to <typeparamref name="TReceiver"/>.
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TReceiver"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class Messenger<TSender, TReceiver, TMessage> : Messenger
    {

#pragma warning disable CS8601 // Possible null reference assignment.
        private TMessage _message = default;
#pragma warning restore CS8601 // Possible null reference assignment.

        /// <summary>
        /// <c>True</c> if the message is preserved after retrival. <c>False</c> if the message get lost.
        /// </summary>
        public abstract bool Preserve { get; }

        /// <summary>
        /// Stores the message that has to be sent to the receiver.
        /// </summary>
        /// <param name="sender">The object that shares the data.</param>
        /// <returns>The message itself to allow chaing calls.</returns>
        public TMessage Send(TSender sender)
        {
            MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Sending);

            _message = GetMessage(sender);

            return _message;
        }

        /// <summary>
        /// Retrives the message sent.
        /// </summary>
        /// <param name="receiver"></param>
        /// <returns>The message sent.</returns>
        /// <exception cref="MessageNullException"></exception>
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "The reciever is needed to ensure that not anyone is allawed to retrive the message. You must at least own the receiver.")]
        public TMessage Receive(TReceiver receiver)
        {
            MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Receiving);

            if (_message is null) 
            {
                MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Failed);
                throw new MessageNullException(Preserve ? $"{GetType().Name} is never been sent." : $"{GetType().Name} has been already retrived or is never been sent."); 
            }

            if (Preserve) return _message;

            TMessage message = _message;
#pragma warning disable CS8601 // Possible null reference assignment.
            _message = default;
#pragma warning restore CS8601 // Possible null reference assignment.

            return message;
        }

        /// <summary>
        /// Specify how to retrive a message from the sender.
        /// </summary>
        /// <param name="sender">Object that shares data</param>
        /// <returns>The data shared</returns>
        protected abstract TMessage GetMessage(TSender sender);

    }

    /// <summary>
    /// Implement this object to publish a <typeparamref name="TMessage"/> from <typeparamref name="TSender"/> to anyone.
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class Messenger<TSender, TMessage> : Messenger
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        private TMessage _message = default;
#pragma warning restore CS8601 // Possible null reference assignment.

        /// <summary>
        /// Stores the message that has to be sent to the receivers.
        /// </summary>
        /// <param name="sender">The object that shares the data.</param>
        /// <returns>The message itself to allow chaing calls.</returns>
        public TMessage Send(TSender sender)
        {
            MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Sending);

            _message = GetMessage(sender);

            return _message;
        }

        /// <summary>
        /// Retrives the message sent.
        /// </summary>
        /// <returns>The message sent.</returns>
        public TMessage Receive()
        {
            MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Receiving);

            if ( _message is null)
            {
                MessageHub.Default.OnMessengerEvent(this, MessengerEvent.Failed);

                throw new MessageNullException($"{GetType().Name} is never been sent.")
            }

            return _message;
        }

        /// <summary>
        /// Specify how to retrive a message from the sender.
        /// </summary>
        /// <param name="sender">Object that shares data</param>
        /// <returns>The data shared</returns>
        protected abstract TMessage GetMessage(TSender sender);

    }
}
