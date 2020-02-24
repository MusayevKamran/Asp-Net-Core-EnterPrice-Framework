using System;
using System.Collections;
using System.Collections.Generic;
using App.Common.Enums;

namespace App.Common.Models
{
    /// <summary>Represents the result of validating an object.</summary>
    [Serializable]
    public class ValidationResults : IEnumerable<ValidationResult>, IEnumerable
    {
        private List<ValidationResult> validationResults;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults" /> class with the section name.</para>
        /// </summary>
        public ValidationResults()
        {
            this.validationResults = new List<ValidationResult>();
        }

        /// <summary>
        /// <para>Adds a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult" />.</para>
        /// </summary>
        /// <param name="validationResult">The validation result to add.</param>
        public void AddResult(ValidationResult validationResult)
        {
            this.validationResults.Add(validationResult);
        }

        /// <summary>
        /// <para>Adds all the <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult" /> instances from <paramref name="sourceValidationResults" />.</para>
        /// </summary>
        /// <param name="sourceValidationResults">The source for validation results to add.</param>
        public void AddAllResults(
            IEnumerable<ValidationResult> sourceValidationResults)
        {
            this.validationResults.AddRange(sourceValidationResults);
        }

        /// <summary>
        /// Returns a new instance of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults" /> that includes the results from the receiver that
        /// match the provided tag names.
        /// </summary>
        /// <param name="tagFilter">The indication of whether to include or ignore the matching results.</param>
        /// <param name="tags">The list of tag names to match.</param>
        /// <returns>A <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResults" /> containing the filtered results.</returns>
        public ValidationResults FindAll(TagFilter tagFilter, params string[] tags)
        {
            if (tags == null)
                tags = new string[1];
            ValidationResults validationResults = new ValidationResults();
            foreach (ValidationResult validationResult in (IEnumerable<ValidationResult>)this)
            {
                bool flag = false;
                foreach (string tag in tags)
                {
                    if (tag == null && validationResult == null || tag != null && tag.Equals(validationResult.Tag))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag ^ tagFilter == TagFilter.Ignore)
                    validationResults.AddResult(validationResult);
            }

            return validationResults;
        }

        /// <summary>
        /// Gets the indication of whether the validation represented by the receiver was successful.
        /// </summary>
        /// <remarks>
        /// An unsuccessful validation will be represented by a <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult" /> instance with
        /// <see cref="T:Microsoft.Practices.EnterpriseLibrary.Validation.ValidationResult" /> elements, regardless of these elements' tags.
        /// </remarks>
        public bool IsValid
        {
            get { return this.validationResults.Count == 0; }
        }

        /// <summary>Gets the count of results.</summary>
        public int Count
        {
            get { return this.validationResults.Count; }
        }

        IEnumerator<ValidationResult> IEnumerable<ValidationResult>.GetEnumerator()
        {
            return (IEnumerator<ValidationResult>)this.validationResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.validationResults.GetEnumerator();
        }
    }
}