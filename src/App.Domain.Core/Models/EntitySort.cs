using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using App.Domain.Core.Interfaces;
using App.Domain.Core.Models.Rule;
using App.Domain.Core.Utilities;

namespace App.Domain.Core.Models
{

    public class EntitySort<T> : IEntitySort<T>
    {
        /// <summary>
        /// Collection of sortable field names and bool values
        /// </summary>
        public List<ISortRule> SortRules { get; set; }

        /// <summary>
        /// Creates a strongly typed field to sort
        /// </summary>
        public EntitySort() { }

        /// <summary>
        /// Creates a strongly typed field to sort
        /// </summary>
        /// <param name="fieldName">Field (using . for inner fields)</param>
        /// <param name="isAscending">Whether to sort ascending (default true)</param>
        public EntitySort(string fieldName, bool isAscending = true)
        {
            Add(fieldName, isAscending);
        }

        /// <summary>
        /// Adds a strongly typed field to sort
        /// </summary>
        /// <param name="fieldName">Field (using . for inner fields)</param>
        /// <param name="isAscending">Whether to sort ascending (default true)</param>
        /// <returns>Current instance</returns>
        public EntitySort<T> Add(string fieldName, bool isAscending = true)
        {
            if (SortRules == null)
                SortRules = new List<ISortRule>();

            // TODO: should we check for duplicates or leave it to the user?
            SortRules.Add(new SortRule() { Name = fieldName, IsAscending = isAscending });

            return this;
        }

        /// <summary>
        /// Creates a strongly typed field to sort
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="isAscending">Whether to sort ascending (default true)</param>
        public EntitySort(Expression<Func<T, object>> field, bool isAscending = true)
        {
            Add(field, isAscending);
        }

        /// <summary>
        /// Adds a strongly typed field to sort
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="isAscending">Whether to sort ascending (default true)</param>
        /// <returns>Current instance</returns>
        public EntitySort<T> Add(Expression<Func<T, object>> field, bool isAscending = true)
        {
            Add(field.GetExpressionMemberName(), isAscending);

            return this;
        }

        /// <summary>
        /// Retrieves a list of SortRules.
        /// Useful if you want to modify a rule and reuse the EntitySort with the new rule again.
        /// </summary>
        /// <returns>A list of modifiable sorts</returns>
        public virtual List<SortRule> GetAllSortRules()
        {
            var collector = new List<SortRule>();
            foreach (var f in SortRules)
            {
                // for now, this is the only rule we know
                if (f is SortRule sortRule)
                    collector.Add(sortRule);
                else
                    throw new InvalidOperationException("Unknown implementation of ISortRule. You must override GetAllSortRules to process custom ISortRules.");
            }

            return collector;
        }
    }
}
