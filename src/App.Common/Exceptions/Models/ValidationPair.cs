using System;
using System.Runtime.Serialization;

namespace App.Common.Exceptions.Models
{
    [Serializable]
    public class ValidationPair
    {
        /// <summary>
        /// Validation key (field) name
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Validation message
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Show validation message to user?
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }
    }
}
