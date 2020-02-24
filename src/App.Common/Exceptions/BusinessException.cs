using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Business exception.
    /// Use for business rule checks which CANNOT be fixed by user and which sould get admin's attention.
    /// This exception will cause error report emails. 
    /// </summary>
    [Serializable]
    public class BusinessException : CorrelatedException
    {
        /// <summary>
        /// Creates a new business exception
        /// </summary>
        /// <param name="message">Error message</param>
        public BusinessException(string message) : base(message) { }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
