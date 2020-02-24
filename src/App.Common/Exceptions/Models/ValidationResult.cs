using System;
using System.Collections.Generic;

namespace App.Common.Exceptions.Models
{
    /// <summary>Represents the result of an atomic validation.</summary>
    [Serializable]
    public class ValidationResult
    {
        private static readonly IEnumerable<ValidationResult> NoNestedValidationResults = (IEnumerable<ValidationResult>)new ValidationResult[0];
        private string _message;
        private string _key;
        private string _tag;
        [NonSerialized]
        private object _target;
        [NonSerialized]
        private Validator _validator;
        private IEnumerable<ValidationResult> _nestedValidationResults;

        /// <summary>Initializes this object with a message.</summary>
        public ValidationResult(
          string message,
          object target,
          string key,
          string tag,
          Validator validator)
          : this(message, target, key, tag, validator, ValidationResult.NoNestedValidationResults)
        {
        }

        /// <summary>Initializes this object with a message.</summary>
        public ValidationResult(
          string message,
          object target,
          string key,
          string tag,
          Validator validator,
          IEnumerable<ValidationResult> nestedValidationResults)
        {
            this._message = message;
            this._key = key;
            this._target = target;
            this._tag = tag;
            this._validator = validator;
            this._nestedValidationResults = nestedValidationResults;
        }

        /// <summary>
        /// Gets a name describing the location of the validation result.
        /// </summary>
        public string Key => this._key;

        /// <summary>Gets a message describing the failure.</summary>
        public string Message => this._message;

        /// <summary>Gets a value characterizing the result.</summary>
        public string Tag => this._tag;

        /// <summary>
        /// Gets the object to which the validation rule was applied.
        /// </summary>
        /// <remarks>
        /// This object might not be the object for which validation was requested initially.
        /// </remarks>
        public object Target => this._target;

        /// <summary>
        /// Gets the validator that performed the failing validation.
        /// </summary>
        public Validator Validator => this._validator;

        /// <summary>
        /// Gets the nested validation results for a composite failed validation.
        /// </summary>
        public IEnumerable<ValidationResult> NestedValidationResults => this._nestedValidationResults;
    }
}
