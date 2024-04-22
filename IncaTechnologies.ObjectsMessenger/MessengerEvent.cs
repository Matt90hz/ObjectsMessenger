using IncaTechnologies.ObjectsMessenger;

namespace IncaTecnologies.ObjectsMessenger
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
        /// Before receiving something.
        /// </summary>
        Receiving, 
        /// <summary>
        /// After receiving something.
        /// </summary>
        Failed 
    }

}
