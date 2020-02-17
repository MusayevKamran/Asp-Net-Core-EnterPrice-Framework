using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using App.Domain.Core.Enums;
using App.Domain.Core.Helpers;
using App.Domain.Core.Interfaces;
using App.Domain.Core.Models.Rule;
using App.Domain.Core.Utilities;

namespace App.Domain.Core.Models
{
    public class EntityFilter<T> : IEntityFilter<T>
    {
        /// <summary>
        /// Ordered collection of Filter rules for single fields or composite groups
        /// </summary>
        public List<IFilterRule> Filters { get; set; }

        /// <summary>
        /// Creates initial empty filter
        /// </summary>
        public EntityFilter() { }

        /// <summary>
        /// Creates a filter with initial rule
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        public EntityFilter(Expression<Func<T, object>> field,
            object fieldValue, FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false)
        {
            Add(field, fieldValue, FilterCombination.None, filterMatch, isCaseSensitive);
        }

        /// <summary>
        /// Adds a field to filter by
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterCombination">AND/OR combination type</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <param name="allowSilentCreation">Can add new rule if initial rule has not been added yet</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> Add(Expression<Func<T, object>> field,
            object fieldValue, FilterCombination filterCombination = FilterCombination.None,
            FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false,
            bool allowSilentCreation = false)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            // if it's null, we don't care about type - it will become NULL DB constant anyway
            if (fieldValue == null)
            {
                // we accept only Equal
                if (filterMatch != FilterMatch.Equal && filterMatch != FilterMatch.NotEqual)
                    throw new ArgumentException("Null values can be compared only with Equal and NotEqual operators", nameof(filterMatch));
            }
            else
            {
                if (!IsSimpleType(fieldValue.GetType()))
                    throw new ArgumentException("Only primitive types can be used to filter fields", nameof(fieldValue));

                if (fieldValue is string)
                {
                    // Linq does not allow string comparisons
                    if (new[] { FilterMatch.Greater, FilterMatch.GreaterOrEqual, FilterMatch.Less, FilterMatch.LessOrEqual }
                        .Contains(filterMatch))
                        throw new ArgumentException("Cannot use Greater and Less expressions with string values", nameof(filterMatch));
                }
                else
                {
                    if (isCaseSensitive)
                        throw new ArgumentException("Cannot use case sensitive with non-string values", nameof(isCaseSensitive));

                    if (filterMatch == FilterMatch.Like || filterMatch == FilterMatch.StartsWith || filterMatch == FilterMatch.EndsWith)
                        throw new ArgumentException("Cannot use Like/StartsWith/EndsWith with non-string values", nameof(filterMatch));
                }
            }

            var fieldName = field.GetExpressionMemberName();

            ValidateOrCreate(filterCombination, allowSilentCreation);

            Filters.Add(new FilterRule
            {
                Name = fieldName,
                Value = fieldValue,
                // fix the first item if we allowed adding to an empty list
                FilterCombination = Filters.Count > 0 ? filterCombination : FilterCombination.None,
                FilterMatch = filterMatch,
                IsCaseSensitive = isCaseSensitive
            });

            return this;
        }

        // shorthands with validation

        /// <summary>
        /// Ands a field to filter by
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> And(Expression<Func<T, object>> field,
            object fieldValue, FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false)
        {
            return Add(field, fieldValue, FilterCombination.And, filterMatch, isCaseSensitive, true);
        }

        /// <summary>
        /// Ors a field to filter by
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> Or(Expression<Func<T, object>> field,
            object fieldValue, FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false)
        {
            return Add(field, fieldValue, FilterCombination.Or, filterMatch, isCaseSensitive, true);
        }

        // shorthands follow with null values check - useful for filtering from MVC search models


        /// <summary>
        /// Ands a field to filter by, but only if the fieldValue is not null.
        /// The filter is created silently if it is not created yet.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> AndIfNotNull(Expression<Func<T, object>> field,
            object fieldValue, FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false)
        {
            return fieldValue != null ? Add(field, fieldValue, FilterCombination.And, filterMatch, isCaseSensitive, true) : this;
        }

        /// <summary>
        /// Ors a field to filter by, but only if the fieldValue is not null.
        /// The filter is created silently if it is not created yet.
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="fieldValue">Field value</param>
        /// <param name="filterMatch">How to compare</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> OrIfNotNull(Expression<Func<T, object>> field,
            object fieldValue, FilterMatch filterMatch = FilterMatch.Equal, bool isCaseSensitive = false)
        {
            return fieldValue != null ? Add(field, fieldValue, FilterCombination.Or, filterMatch, isCaseSensitive, true) : this;
        }


        // shorthands follow with C# operator expressions instead of pure member expressions

        /// <summary>
        /// Creates a filter with initial rule using C# equals/greater/less expressions. Does not support Like.
        /// </summary>
        /// <param name="fieldRule">Field rule. Must be in a form "field (=&lt;&gt;) constant".</param>
        /// <param name="isCaseSensitive">Is case sensitive rule (for string equals only)</param>
        public EntityFilter(Expression<Func<T, bool>> fieldRule, bool isCaseSensitive = false)
        {
            if (fieldRule == null)
                throw new ArgumentNullException(nameof(fieldRule));

            // solve the rule
            var field = fieldRule.GetExpressionMember();
            var fieldValue = fieldRule.GetExpressionConstant();

            var filterMatch = EntityFilterHelpers.ExpressionNodeTypeToFilterMatch(fieldRule.GetExpressionOperation());

            Add(field, fieldValue, FilterCombination.None, filterMatch, isCaseSensitive);
        }

        /// <summary>
        /// Ands a field to filter using C# equals/greater/less expressions. Does not support Like.
        /// </summary>
        /// <param name="fieldRule">Field rule. Must be in a form "field (=&lt;&gt;) constant".</param>
        /// <param name="isCaseSensitive">Is case sensitive rule (for string equals only)</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> And(Expression<Func<T, bool>> fieldRule, bool isCaseSensitive = false)
        {
            if (fieldRule == null)
                throw new ArgumentNullException(nameof(fieldRule));

            // solve the rule
            var field = fieldRule.GetExpressionMember();
            var fieldValue = fieldRule.GetExpressionConstant();

            var filterMatch = EntityFilterHelpers.ExpressionNodeTypeToFilterMatch(fieldRule.GetExpressionOperation());

            return Add(field, fieldValue, FilterCombination.And, filterMatch, isCaseSensitive, true);
        }

        /// <summary>
        /// Ors a field to filter using C# equals/greater/less expressions. Does not support Like.
        /// </summary>
        /// <param name="fieldRule">Field rule. Must be in a form "field (=&lt;&gt;) constant".</param>
        /// <param name="isCaseSensitive">Is case sensitive rule (for string equals only)</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> Or(Expression<Func<T, bool>> fieldRule, bool isCaseSensitive = false)
        {
            if (fieldRule == null)
                throw new ArgumentNullException(nameof(fieldRule));

            // solve the rule
            var field = fieldRule.GetExpressionMember();
            var fieldValue = fieldRule.GetExpressionConstant();

            var filterMatch = EntityFilterHelpers.ExpressionNodeTypeToFilterMatch(fieldRule.GetExpressionOperation());

            return Add(field, fieldValue, FilterCombination.Or, filterMatch, isCaseSensitive, true);
        }

        /// <summary>
        /// Ands a field to filter by, but only if the fieldValue is not null.
        /// The filter is created silently if it is not created yet.
        /// </summary>
        /// <param name="fieldRule">Field rule. Must be in a form "field (=&lt;&gt;) constant".</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> AndIfNotNull(Expression<Func<T, bool>> fieldRule, bool isCaseSensitive = false)
        {
            if (fieldRule == null)
                throw new ArgumentNullException(nameof(fieldRule));

            // solve the rule
            var field = fieldRule.GetExpressionMember();
            var fieldValue = fieldRule.GetExpressionConstant();

            var filterMatch = EntityFilterHelpers.ExpressionNodeTypeToFilterMatch(fieldRule.GetExpressionOperation());

            return fieldValue != null ? Add(field, fieldValue, FilterCombination.And, filterMatch, isCaseSensitive, true) : this;
        }

        /// <summary>
        /// Ors a field to filter by, but only if the fieldValue is not null.
        /// The filter is created silently if it is not created yet.
        /// </summary>
        /// <param name="fieldRule">Field rule. Must be in a form "field (=&lt;&gt;) constant".</param>
        /// <param name="isCaseSensitive">Is case sensitive rule</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> OrIfNotNull(Expression<Func<T, bool>> fieldRule, bool isCaseSensitive = false)
        {
            if (fieldRule == null)
                throw new ArgumentNullException(nameof(fieldRule));

            // solve the rule
            var field = fieldRule.GetExpressionMember();
            var fieldValue = fieldRule.GetExpressionConstant();

            var filterMatch = EntityFilterHelpers.ExpressionNodeTypeToFilterMatch(fieldRule.GetExpressionOperation());

            return fieldValue != null ? Add(field, fieldValue, FilterCombination.Or, filterMatch, isCaseSensitive, true) : this;
        }


        // methods for filter hierarchy follow

        /// <summary>
        /// Creates a filter group with initial rule
        /// </summary>
        /// <param name="filterGroup">Single group of filters</param>
        public EntityFilter(EntityFilter<T> filterGroup)
        {
            Add(filterGroup);
        }

        /// <summary>
        /// Adds a filter group to filter by
        /// </summary>
        /// <param name="filterGroup">Single group of filters</param>
        /// <param name="filterCombination">AND/OR combination type</param>
        /// <param name="allowSilentCreation">Can add new rule if initial rule has not been added yet</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> Add(EntityFilter<T> filterGroup,
            FilterCombination filterCombination = FilterCombination.None,
            bool allowSilentCreation = false)
        {
            if (filterGroup == null)
                throw new ArgumentNullException(nameof(filterGroup));

            ValidateOrCreate(filterCombination, allowSilentCreation);

            Filters.Add(new CompositeFilterRule
            {
                Filters = filterGroup.Filters,
                // fix the first item if we allowed adding to an empty list
                FilterCombination = Filters.Count > 0 ? filterCombination : FilterCombination.None
            });

            return this;
        }

        // shorthands with validation

        /// <summary>
        /// Ands a filter group to filter by
        /// </summary>
        /// <param name="filterGroup">Single group of filters</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> And(EntityFilter<T> filterGroup)
        {
            return Add(filterGroup, FilterCombination.And, true);
        }

        /// <summary>
        /// Ors a filter group to filter by
        /// </summary>
        /// <param name="filterGroup">Single group of filters</param>
        /// <returns>Current instance</returns>
        public EntityFilter<T> Or(EntityFilter<T> filterGroup)
        {
            return Add(filterGroup, FilterCombination.Or, true);
        }

        /// <summary>
        /// Recursively retrieves a flat list of FilterRules.
        /// Useful if you want to modify a rule and reuse the EntityFilter with the new rule again.
        /// </summary>
        /// <returns>A flattened list of filters</returns>
        public virtual List<FilterRule> GetAllFlattenedFilterRules()
        {
            var collector = new List<FilterRule>();

            GetFilterRulesInner(collector, Filters);

            return collector;
        }

        private void GetFilterRulesInner(ICollection<FilterRule> collector, List<IFilterRule> rootFilters)
        {
            if (rootFilters == null) throw new ArgumentNullException(nameof(rootFilters));
            foreach (var f in rootFilters)
            {
                switch (f)
                {
                    // search recursively
                    case FilterRule filterRule:
                        collector.Add(filterRule);
                        break;
                    case CompositeFilterRule filterRule:
                        GetFilterRulesInner(collector, filterRule.Filters);
                        break;
                }
            }
        }

        private void ValidateOrCreate(FilterCombination filterCombination,
            bool allowSilentCreation)
        {
            if (Filters == null)
            {
                if (filterCombination != FilterCombination.None && !allowSilentCreation)
                    throw new ArgumentException("This filter has no rules. You must use None filter combination when adding the first rule.", nameof(filterCombination));

                Filters = new List<IFilterRule>();
            }
            else
            {
                if (filterCombination == FilterCombination.None)
                    throw new ArgumentException("This filter has some rules. You must use And, Or to add new rules.", nameof(filterCombination));
            }
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsValueType ||
                type.IsPrimitive ||
                new[] {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
                Convert.GetTypeCode(type) != TypeCode.Object;
        }
    }

}
