using System;

namespace IncaTechnologies.ObjectsMessenger.Exceptions
{
    /// <summary>
    /// Exception thrown when a message that is not preserved is retrived more than once.
    /// </summary>
    public sealed class MessageAlreadyReceivedException : Exception
    {
        internal MessageAlreadyReceivedException() { }

        internal MessageAlreadyReceivedException(string message) : base(message) { }

        internal MessageAlreadyReceivedException(string message, Exception inner) : base(message, inner) { }
    }
}