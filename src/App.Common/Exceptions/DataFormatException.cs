using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Invalid data exception (to distinguish our own invalid data Exception from the .NET invalid data Exception).
    /// Mostly for exceptions related to 3rd party input data (files, services etc.).
    /// </summary>
    [Serializable]
    public class DataFormatException : CorrelatedException
    {
        /// <summary>
        /// Creates a new data format exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="data">Error data</param>
        /// <param name="reason">Error reason</param>
        public DataFormatException(string message, string data, string reason) : base(message)
        {
            AssociatedData = data;
            Reason = reason;
        }

        /// <summary>
        /// Data associated with this fault
        /// </summary>
        public string AssociatedData { get; set; }

        /// <summary>
        /// Reason of this fault (see the throwing class to find out all the possible reason strings)
        /// </summary>
        public string Reason { get; set; }

        protected DataFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.AssociatedData = (string)info.GetValue("AssociatedData", typeof(string));
            this.Reason = (string)info.GetValue("Reason", typeof(string));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AssociatedData", this.AssociatedData);
            info.AddValue("Reason", this.Reason);

            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Converts this object to string
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}; Data: {AssociatedData}, Reason: {Reason}";
        }
    }
}

