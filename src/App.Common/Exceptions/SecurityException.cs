using App.Common.Utilities;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Custom security exception
    /// </summary>
    [Serializable]
    public class SecurityException : CorrelatedException
    {
        /// <summary>
        /// Creates a new security exception
        /// </summary>
        /// <param name="message">Security error message</param>
        /// <param name="errorType">Security error type</param>
        public SecurityException(string message, SecurityErrorType errorType)
            : base(message)
        {
            ErrorType = errorType;
        }

        /// <summary>
        /// Security fault type
        /// </summary>
        public SecurityErrorType ErrorType { get; private set; }

        protected SecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ErrorType = (SecurityErrorType)info.GetValue("ErrorType", typeof(SecurityErrorType));
        }

        // only code on the local computer is granted this SerializationFormatter permission
        // see http://msdn.microsoft.com/LV-LV/library/ek7af9ck(v=vs.71).aspx for more info
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ErrorType", this.ErrorType);
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Converts this object to string
        /// </summary>
        public override string ToString()
        {
            return string.Format("Error type: {1};\r\n {0}", base.ToString(), EnumUtils.StringValueOf(ErrorType));
        }
    }


    /// <summary>
    /// Security error type
    /// </summary>
    public enum SecurityErrorType
    {
        /// <summary>
        /// Session expires because of timeouts
        /// </summary>
        SessionExpired = 10,

        /// <summary>
        /// Session expires because of detected client IP changes
        /// </summary>
        SessionClientChange = 12,

        /// <summary>
        /// No valid authentication found
        /// </summary>
        NotAuthenticated = 2,

        /// <summary>
        /// Context identity is not allowed to execute the action
        /// </summary>
        NotAuthorised = 3,

        /// <summary>
        /// For various validation issues (password validation etc.)
        /// </summary>
        PolicyNotSatisfied = 41,

        /// <summary>
        /// Thrown if user name contains bad symbols (when setting username)
        /// </summary>
        InvalidUsername = 42,

        /// <summary>
        /// Thrown if user name is not unique
        /// </summary>
        DuplicateUsername = 43,

        /// <summary>
        /// Thrown if password hash does not match (when changing password)
        /// </summary>
        InvalidPassword = 51,

        /// <summary>
        /// Thrown on user login to report about expired password
        /// </summary>
        TemporaryPasswordExpired = 52,

        /// <summary>
        /// Thrown on authorisation if user has a temporary password or an expired permanent password
        /// </summary>
        MustChangePassword = 53,

        /// <summary>
        /// Invalid password count exceeded - used as a reason for the corresponding lock
        /// </summary>
        InvalidPasswordCountLimit = 54,

        /// <summary>
        /// Not activated - used as a reason for the corresponding lock
        /// </summary>
        NotActivated = 55,

        /// <summary>
        /// Thrown on user login to vaguely report about wrong credentials without disposing details
        /// </summary>
        InvalidCredentials = 61,

        /// <summary>
        /// Thrown on user login to report about locked login and reason
        /// </summary>
        LoginLocked = 71,

        /// <summary>
        /// Thrown if the user is trying to lock / unlock status which is already in a matching state
        /// </summary>
        LoginLockInconsistency = 72,

        /// <summary>
        /// Thrown if the user is trying to modify his own permissions
        /// </summary>
        CannotModifyOwnPermissions = 81,

        /// <summary>
        /// Thrown if trying to add duplicate permissions
        /// </summary>
        DuplicatePermission = 82,

        /// <summary>
        /// Thrown if e-mail is not unique
        /// </summary>
        DuplicateEmail = 91,

        /// <summary>
        /// Thrown if e-mail is not valid
        /// </summary>
        InvalidEmail = 92,

        /// <summary>
        /// Thrown if other noncritical input values are not valid (too long etc.)
        /// </summary>
        InvalidFieldLength = 93,

        /// <summary>
        /// Thrown if verification data not valid or expired
        /// </summary>
        InvalidVerification = 101,

        /// <summary>
        /// Thrown if service connection versions mismatch
        /// </summary>
        UnexpectedVersion = 400,

        /// <summary>
        /// Used for other unspecified cases and also to mask errors for possible attackers
        /// </summary>
        InvalidOperation = 505
    }
}

