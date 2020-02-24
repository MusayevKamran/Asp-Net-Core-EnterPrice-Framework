using App.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace App.Common.Exceptions
{
    /// <summary>
    /// Business validation exception. 
    /// Use for input data validation errors and also for business rule checks which can be fixed by user. 
    /// This exception will not cause error report emails. 
    /// </summary>
    [Serializable]
    public class BusinessValidationException : CorrelatedException
    {
        /// <summary>
        /// Creates a business validation exception class
        /// </summary>
        public BusinessValidationException()
            : base(string.Empty)
        {
            ValidationResults = new ValidationResults();
        }

        /// <summary>
        /// Creates a business validation exception class
        /// </summary>
        /// <param name="validationPairs">Validation results</param>
        public BusinessValidationException(ValidationPair[] validationPairs)
            : base(string.Empty)
        {
            ValidationPairs = validationPairs;
        }

        /// <summary>
        /// Creates a business validation exception class
        /// </summary>
        /// <param name="validationResults">Validation results</param>
        public BusinessValidationException(ValidationResults validationResults)
            : base(string.Empty)
        {
            ValidationResults = validationResults;
        }

        /// <summary>
        /// Creates a business validation exception class passing a single validation entry
        /// </summary>
        /// <param name="errorMessage">Validation message</param>
        /// <param name="errorKey">Validation key (entity field)</param>
        /// <param name="isVisible">Show error message to user?</param>
        public BusinessValidationException(string errorMessage, string errorKey, bool isVisible = true)
            : base(string.Empty)
        {
            ValidationResults = new ValidationResults();
            ValidationResults.AddResult(new ValidationResult(errorMessage, null, errorKey, isVisible.ToString(), null));
        }

        public BusinessValidationException(Dictionary<string, string[]> modelState)
            : base(string.Empty)
        {
            ValidationResults = new ValidationResults();

            foreach (var e in modelState)
            {
                StringBuilder sb = new StringBuilder(modelState.Count * 20);

                if (e.Value != null)
                {
                    foreach (var value in e.Value)
                        sb.AppendLine(value);
                }

                ValidationResults.AddResult(new ValidationResult(sb.ToString(), null, e.Key, string.Empty, null));
            }
        }

        /// <summary>
        /// Adds a validation entry to this exception
        /// </summary>
        /// <param name="errorMessage">Validation message</param>
        /// <param name="errorKey">Validation key (entity field)</param>
        public BusinessValidationException AddErrorMessage(string errorMessage, string errorKey)
        {
            ValidationResults.AddResult(new ValidationResult(errorMessage, null, errorKey,
                string.Empty, null));
            return this;
        }


        /// <summary>
        /// Validation results
        /// </summary>
        public ValidationResults ValidationResults { get; private set; }

        /// <summary>
        /// Simulated with validation string results, used for serialisation
        /// </summary>
        public ValidationPair[] ValidationPairs
        {
            get
            {
                if (ValidationResults == null)
                    return null;

                return ValidationResults.Select(res => new ValidationPair { Key = res.Key, Message = res.Message, IsVisible = Convert.ToBoolean(res.Tag) }).ToArray();
            }

            set
            {
                ValidationResults = new ValidationResults();

                if (value == null)
                    return;

                foreach (var res in value)
                {
                    ValidationResults.AddResult(new ValidationResult(res.Message ?? string.Empty,
                                null,
                                res.Key ?? string.Empty, res.IsVisible.ToString(), null));
                };
            }
        }

        protected BusinessValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.ValidationPairs = (ValidationPair[])info.GetValue("ValidationResultPairs", typeof(ValidationPair[]));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ValidationResultPairs", this.ValidationPairs);
            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Converts this object to string
        /// </summary>
        public override string ToString()
        {
            return string.Format("Validation results:\n{1}\n{0}", base.ToString(), ValidationsToString());
        }

        /// <summary>
        /// Converts this object validation errors to string
        /// </summary>
        public string ValidationsToString()
        {
            if (ValidationResults != null)
            {
                StringBuilder sb = new StringBuilder(ValidationPairs.Length * 20);
                foreach (var valRes in ValidationResults)
                    sb.AppendLine((valRes.Key ?? "") + ": " + (valRes.Message ?? ""));

                return sb.ToString();
            }

            return string.Empty;
        }
    }
}
