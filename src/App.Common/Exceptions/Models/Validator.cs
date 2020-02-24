using System;
using System.Collections.Generic;
using System.Globalization;

namespace App.Common.Exceptions.Models
{
    /// <summary>Represents logic used to validate an object.</summary>
    public abstract class Validator
    {
        private string _messageTemplate;
        private string _tag;

        /// <param name="messageTemplate">The template to use when logging validation results, or <see langword="null" /> we the
        /// default message template is to be used.</param>
        /// <param name="tag">The tag to set when logging validation results, or <see langword="null" />.</param>
        protected Validator(string messageTemplate, string tag)
        {
            _messageTemplate = messageTemplate;
            _tag = tag;
        }

        /// <summary>
        /// Applies the validation logic represented by the receiver on an object,
        /// returning the validation results.
        /// </summary>
        /// <param name="target">The object to validate.</param>
         public ValidationResults Validate(object target)
        {
            var validationResults = new ValidationResults();
            DoValidate(target, target, null, validationResults);
            return validationResults;
        }

        /// <summary>
        /// Applies the validation logic represented by the receiver on an object,
        /// adding the validation results to <paramref name="validationResults" />.
        /// </summary>
        /// <param name="target">The object to validate.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        public void Validate(object target, ValidationResults validationResults)
        {
            if (validationResults == null)
                throw new ArgumentNullException(nameof(validationResults));
            DoValidate(target, target, null, validationResults);
        }

        /// <summary>Implements the validation logic for the receiver.</summary>
        /// <param name="objectToValidate">The object to validate.</param>
        /// <param name="currentTarget">The object on the behalf of which the validation is performed.</param>
        /// <param name="key">The key that identifies the source of <paramref name="objectToValidate" />.</param>
        /// <param name="validationResults">The validation results to which the outcome of the validation should be stored.</param>
        /// <remarks>
        /// Subclasses must provide a concrete implementation the validation logic.
        /// </remarks>
        public abstract void DoValidate(
          object objectToValidate,
          object currentTarget,
          string key,
          ValidationResults validationResults);

        /// <summary>
        /// Logs a validation result to <paramref name="validationResults" />.
        /// </summary>
        /// <param name="validationResults">The validation results to which the new result should be stored.</param>
        /// <param name="message">The message that describes the result.</param>
        /// <param name="target">The object to which the result is related to.</param>
        /// <param name="key">The key that identifies how the result relates to the target.</param>
        protected void LogValidationResult(
          ValidationResults validationResults,
          string message,
          object target,
          string key)
        {
            if (validationResults == null)
                throw new ArgumentNullException(nameof(validationResults));
            validationResults.AddResult(new ValidationResult(message, target, key, Tag, this));
        }

        /// <summary>
        /// Logs a validation result to <paramref name="validationResults" />.
        /// </summary>
        /// <param name="validationResults">The validation results to which the new result should be stored.</param>
        /// <param name="message">The message that describes the result.</param>
        /// <param name="target">The object to which the result is related to.</param>
        /// <param name="key">The key that identifies how the result relates to the target.</param>
        /// <param name="nestedValidationResults"></param>
        protected void LogValidationResult(
          ValidationResults validationResults,
          string message,
          object target,
          string key,
          IEnumerable<ValidationResult> nestedValidationResults)
        {
            if (validationResults == null)
                throw new ArgumentNullException(nameof(validationResults));
            validationResults.AddResult(new ValidationResult(message, target, key, Tag, this, nestedValidationResults));
        }

        /// <summary>Gets the message representing a failed validation.</summary>
        /// <param name="objectToValidate">The object for which validation was performed.</param>
        /// <param name="key">The key representing the value being validated for <paramref name="objectToValidate" />.</param>
        /// <returns>The message representing the validation failure.</returns>
        /// <remarks>The default validation maessage formatting provides the object to validate, the key and the tag.<para />
        /// Subclasses may provide additional formatting parameters.
        /// </remarks>
        protected internal virtual string GetMessage(object objectToValidate, string key)
        {
            return string.Format(CultureInfo.CurrentCulture, MessageTemplate, objectToValidate, key, Tag);
        }

        /// <summary>
        /// Gets the message template to use when logging results no message is supplied.
        /// </summary>
        protected abstract string DefaultMessageTemplate { get; }

        /// <summary>
        /// Gets or sets the message template to use when logging results.
        /// </summary>
        public string MessageTemplate
        {
            get => _messageTemplate ?? DefaultMessageTemplate;
            set => _messageTemplate = value;
        }

        /// <summary>Gets a value characterizing the logged result.</summary>
        public string Tag
        {
            get => _tag;
            set => _tag = value;
        }
    }
}
