using App.Domain.Core.Attributes;
using App.Domain.Core.Enums;
using App.Domain.Core.Interfaces;

namespace App.Domain.Core.Models.Rule
{
    /// <summary>
    /// Serializable filter rule structure
    /// </summary>
    [Dto]
    public class FilterRule : IFilterRule
    {
        /// <summary>
        /// String for field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value to compare db fields.
        /// Notice: everything non-POCO you put here must be serializable (marked with Dto attribute).
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// How to combine passed filters (AND / OR), default is AND
        /// </summary>
        public FilterCombination FilterCombination { get; set; }

        /// <summary>
        /// Matching rule
        /// </summary>
        public FilterMatch FilterMatch { get; set; }

        /// <summary>
        /// How to match string values. Will be ignored for other types.
        /// </summary>
        public bool IsCaseSensitive { get; set; }
    }
}
