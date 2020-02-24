using System;
using System.Collections;
using System.Collections.Generic;
using App.Common.Exceptions.Enums;

namespace App.Common.Exceptions.Models
{
    /// <summary>Represents the result of validating an object.</summary>
    [Serializable]
    public class ValidationResults : IEnumerable<ValidationResult>
    {
        private List<ValidationResult> _validationResults;

        public ValidationResults()
        {
            _validationResults = new List<ValidationResult>();
        }


        /// <param name="validationResult">The validation result to add.</param>
        public void AddResult(ValidationResult validationResult)
        {
            _validationResults.Add(validationResult);
        }


        /// <param name="sourceValidationResults">The source for validation results to add.</param>
        public void AddAllResults(
            IEnumerable<ValidationResult> sourceValidationResults)
        {
            _validationResults.AddRange(sourceValidationResults);
        }


        /// <param name="tagFilter">The indication of whether to include or ignore the matching results.</param>
        /// <param name="tags">The list of tag names to match.</param>
         public ValidationResults FindAll(TagFilter tagFilter, params string[] tags)
        {
            if (tags == null)
                tags = new string[1];

            var validationResults = new ValidationResults();
            foreach (var validationResult in this)
            {
                var flag = false;
                foreach (var tag in tags)
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
        public bool IsValid => _validationResults.Count == 0;

        /// <summary>Gets the count of results.</summary>
        public int Count => _validationResults.Count;

        IEnumerator<ValidationResult> IEnumerable<ValidationResult>.GetEnumerator()
        {
            return _validationResults.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _validationResults.GetEnumerator();
        }
    }
}