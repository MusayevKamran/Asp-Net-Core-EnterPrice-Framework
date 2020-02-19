using System.Collections.Generic;

namespace App.Domain.Core.Interfaces
{
    /// <summary>
    /// Interface mostly for dealing with variance issues when passing EntitySort around
    /// </summary>
    public interface IEntitySort<out T>
    {
        /// <summary>
        /// Collection of sortable field names and bool values
        /// </summary>
        List<ISortRule> SortRules { get; set; }
    }
}
