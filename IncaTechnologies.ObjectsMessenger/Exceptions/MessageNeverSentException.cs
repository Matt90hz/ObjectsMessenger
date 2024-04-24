using System;

namespace IncaTechnologies.ObjectsMessenger.Exceptions
{
    /// <summary>
    /// Exception throw when a message is retrived before it is sended
    /// </summary>
    public sealed class MessageNeverSentException : Exception
    {
        internal MessageNeverSentException() { }

        internal MessageNeverSentException(string message) : base(message) { }

        internal MessageNeverSentException(string message, Exception inner) : base(message, inner) { }
    }
}