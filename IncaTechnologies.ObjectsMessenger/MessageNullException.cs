using System;

namespace IncaTecnologies.ObjectsMessenger
{

    /// <summary>
    /// Exception thrown when a message that is not preserved is retrived. Or when a message in never been sent.
    /// </summary>
    public sealed class MessageNullException : Exception
    {
        internal MessageNullException() { }

        internal MessageNullException(string message) : base(message) { }

        internal MessageNullException(string message, Exception inner) : base(message, inner) { }

    }
}