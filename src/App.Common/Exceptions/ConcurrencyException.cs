using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Custom concurrency exception used to indicate that there was a conflict between parallel processes
    /// (e.g. database updates).
    /// </summary>
    [Serializable]
    public class ConcurrencyException : CorrelatedException
    {
        /// <summary>
        /// Creates a new concurrency exception
        /// </summary>
        /// <param name="message">Optional error message</param>
        public ConcurrencyException(string message = null)
            : base(message)
        {

        }

        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        // only code on the local computer is granted this SerializationFormatter permission
        // see http://msdn.microsoft.com/LV-LV/library/ek7af9ck(v=vs.71).aspx for more info
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
