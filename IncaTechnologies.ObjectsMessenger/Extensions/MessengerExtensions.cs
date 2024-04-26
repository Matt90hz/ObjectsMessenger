using IncaTechnologies.ObjectsMessenger;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace IncaTechnologies.ObjectsMessenger
{
    /// <summary>
    /// Utility extensions for <see cref="Messenger"/>.
    /// </summary>
    public static class MessengerExtensions
    {
        /// <summary>
        /// Utility to set a property backing field and send the new value as a message in one line.
        /// <example>
        /// <code>
        /// class MyClass
        /// {
        ///     private readonly ObjectMessenger _objectMessenger;
        ///     private object _myProperty;
        ///     
        ///     public object MyProperty
        ///     {
        ///         get => _myProperty;
        ///         set => _objectMessenger.SetAndSend(this, ref _myProperty, value);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </summary>      
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TReceiver"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="messenger"></param>
        /// <param name="sender"></param>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">Incoming value for the property.</param>

        public static void SetAndSend<TSender, TReceiver, TMessage>(this Messenger<TSender, TReceiver, TMessage> messenger, TSender sender, ref TMessage field, TMessage value)
        {
            field = value;
            messenger.Send(sender);
        }

        /// <summary>
        /// Utility to set a property backing field and send the new value as a message in one line.
        /// <example>
        /// <code>
        /// class MyClass
        /// {
        ///     private readonly ObjectMessenger _objectMessenger;
        ///     private object _myProperty;
        ///     
        ///     public object MyProperty
        ///     {
        ///         get => _myProperty;
        ///         set => _objectMessenger.SetAndSend(this, ref _myProperty, value);
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </summary> 
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="messenger"></param>
        /// <param name="sender"></param>
        /// <param name="field">Reference to the backing field of the property.</param>
        /// <param name="value">Incoming value for the property.</param>
        public static void SetAndSend<TSender, TMessage>(this Messenger<TSender, TMessage> messenger, TSender sender, ref TMessage field, TMessage value)
        {
            field = value;
            messenger.Send(sender);
        }
    }
}
