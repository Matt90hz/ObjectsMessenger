using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using IncaTechnologies.ObjectsMessenger;
using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using IncaTechnologies.ObjectsMessenger.Exceptions;

namespace IncaTechnologies.ObjectsMessenger
{

    /// <summary>
    /// Marker for <see cref="Messenger{TSender, TMessage}"/> and <see cref="Messenger{TSender, TReceiver, TMessage}"/> classes.
    /// </summary>
    public abstract class Messenger 
    {
        /// <summary>
        /// Return an observable series of <see cref="MessengerEvent"/>. The series never calls OnComplete or OnError.
        /// </summary>
        public abstract IObservable<MessengerEvent> Events { get; }

        /// <summary>
        /// A series of all the exception that might occurr during the messenging process.
        /// </summary>
        /// <remarks>
        /// <see cref="MessageAlreadyReceivedException"/>, <see cref="MessageNeverSentException"/>.
        /// </remarks>
        public abstract IObservable<Exception> Errors { get; }
    }

    /// <summary>
    /// Implement this object to allow the transmission of <typeparamref name="TMessage"/> from <typeparamref name="TSender"/> to <typeparamref name="TReceiver"/>.
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TReceiver"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class Messenger<TSender, TReceiver, TMessage> : Messenger
    {
        readonly Subject<MessengerEvent> _events = new Subject<MessengerEvent>();
        readonly Subject<Exception> _errors = new Subject<Exception>();
        private TMessage _message = default!;

        /// <inheritdoc/>
        public sealed override IObservable<MessengerEvent> Events => _events;

        /// <inheritdoc/>
        public sealed override IObservable<Exception> Errors => _errors;

        /// <summary>
        /// <c>True</c> if the message was sended at least once. <c>False</c> if it was never sended.
        /// </summary>
        public bool IsMessageSended { get; private set; } = false;

        /// <summary>
        /// <c>True</c> if the message was received. <c>False</c> if it was never received.
        /// </summary>
        public bool IsMessageReceived { get; private set; } = false;

        /// <summary>
        /// <c>True</c> if the message is preserved after retrival. <c>False</c> if the message get lost.
        /// </summary>
        public abstract bool IsMessagePreserved { get; }

        /// <summary>
        /// Stores the message that has to be sent to the receiver.
        /// </summary>
        /// <param name="sender">The object that shares the data.</param>
        /// <returns>The messenger to chain calls.</returns>
        public Messenger<TSender, TReceiver, TMessage> Send(TSender sender)
        {
            _events.OnNext(MessengerEvent.Sending);

            _message = SendMessage(sender);

            IsMessageSended = true;
            IsMessageReceived = false;

            _events.OnNext(MessengerEvent.Sended);

            return this;
        }

        /// <summary>
        /// Retrives the message sent.
        /// </summary>
        /// <param name="receiver">The object that will receive the data.</param>
        /// <returns>The messenger to chain calls.</returns>
        public Messenger<TSender, TReceiver, TMessage> Receive(TReceiver receiver)
        {
            _events.OnNext(MessengerEvent.Receiving);

            if (IsMessageSended is false) 
            {
                _events.OnNext(MessengerEvent.ReceiveFailed);
                _errors.OnNext(new MessageNeverSentException());
                return this;
            }

            if (!IsMessagePreserved && IsMessageReceived) 
            {
                _events.OnNext(MessengerEvent.ReceiveFailed);
                _errors.OnNext(new MessageAlreadyReceivedException());
                return this;
            }

            ReceiveMessage(receiver, _message);

            IsMessageReceived = true;

            _events.OnNext(MessengerEvent.Received);

            if (IsMessagePreserved) return this;        

            _message = default!;

            return this;
        }

        /// <summary>
        /// Specify how to retrive a message from the sender.
        /// </summary>
        /// <param name="sender">Object that shares data.</param>
        /// <returns>The data shared.</returns>
        protected abstract TMessage SendMessage(TSender sender);

        /// <summary>
        /// Specify how to receive the message.
        /// </summary>
        /// <param name="receiver">Object that receive the data.</param>
        /// <param name="message">Data received.</param>
        protected abstract void ReceiveMessage(TReceiver receiver, TMessage message);

    }

    /// <summary>
    /// Implement this object to publish a <typeparamref name="TMessage"/> from <typeparamref name="TSender"/> to anyone.
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TMessage"></typeparam>
    public abstract class Messenger<TSender, TMessage> : Messenger
    {
        readonly Subject<MessengerEvent> _events = new Subject<MessengerEvent>();
        readonly Subject<Exception> _errors = new Subject<Exception>();
        private TMessage _message = default!;

        /// <inheritdoc/>
        public sealed override IObservable<MessengerEvent> Events => _events;

        /// <summary>
        /// A series of all the exception that might occurr during the messenging process.
        /// </summary>
        /// <remarks>
        /// <see cref="MessageNeverSentException"/>.
        /// </remarks>
        public sealed override IObservable<Exception> Errors => _errors;

        /// <summary>
        /// <see cref="Receive"/> returns this value in case of failure.
        /// </summary>
        public abstract TMessage Default { get; }

        /// <summary>
        /// <c>True</c> if the message was sended at least once. <c>False</c> if it was never sended.
        /// </summary>
        public bool IsMessageSended { get; private set; } = false;

        /// <summary>
        /// Stores the message that has to be sent to the receivers.
        /// </summary>
        /// <param name="sender">The object that shares the data.</param>
        /// <returns>The message itself to allow chaing calls.</returns>
        public Messenger<TSender, TMessage> Send(TSender sender)
        {
            _events.OnNext(MessengerEvent.Sending);

            _message = SendMessage(sender);

            IsMessageSended = true;

            _events.OnNext(MessengerEvent.Sended);

            return this;
        }

        /// <summary>
        /// Retrives the message sent.
        /// </summary>
        /// <returns>The message sent.</returns>
        public TMessage Receive()
        {
            _events.OnNext(MessengerEvent.Receiving);

            if (IsMessageSended is false)
            {
                _events.OnNext(MessengerEvent.ReceiveFailed);
                _errors.OnNext(new MessageNeverSentException());

                return Default;
            }

            return _message;
        }

        /// <summary>
        /// Specify how to retrive a message from the sender.
        /// </summary>
        /// <param name="sender">Object that shares data</param>
        /// <returns>The data shared</returns>
        protected abstract TMessage SendMessage(TSender sender);

    }
}
