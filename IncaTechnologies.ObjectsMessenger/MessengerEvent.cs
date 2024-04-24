using IncaTechnologies.ObjectsMessenger;

namespace IncaTechnologies.ObjectsMessenger
{
    /// <summary>
    /// Events that <see cref="Messenger"/> can trigger.
    /// </summary>
    public enum MessengerEvent 
    { 
        /// <summary>
        /// Before sending something.
        /// </summary>
        Sending,

        /// <summary>
        /// After sending something.
        /// </summary>
        Sended,

        /// <summary>
        /// Before receiving something.
        /// </summary>
        Receiving,

        /// <summary>
        /// After receiving something.
        /// </summary>
        Received,

        /// <summary>
        /// Somethig went wrong
        /// </summary>
        ReceiveFailed 
    }

}
