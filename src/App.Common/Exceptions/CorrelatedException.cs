using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Base exception to contain custom correlation Id
    /// </summary>
    [Serializable]
    public class CorrelatedException : Exception
    {
        /// <summary>
        /// Creates a new correlated exception
        /// </summary>
        /// <param name="message">Error message</param>
        public CorrelatedException(string message) : base(message) { }

        /// <summary>
        /// Creates a new wrapped exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="inner">Exception to wrap</param>
        public CorrelatedException(string message, Exception inner) : base(message, inner) { }

        protected CorrelatedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ActivityId = (string)info.GetValue("ActivityId", typeof(string));
        }

        /// <summary>
        /// Activity Id to help correlate client-server sides
        /// </summary>
        public string ActivityId { get; set; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ActivityId", this.ActivityId);

            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
