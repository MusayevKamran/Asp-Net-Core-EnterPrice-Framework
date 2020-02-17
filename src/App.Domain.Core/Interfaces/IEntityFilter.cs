using System;
using System.Collections.Generic;
using System.Text;

namespace App.Domain.Core.Interfaces
{
    /// <summary>
    /// Interface mostly for dealing with variance issues when passing EntityFilter around
    /// </summary>
    public interface IEntityFilter<out T>
    {
        /// <summary>
        /// Ordered collection of Filter rules for single fields or composite groups
        /// </summary>
        List<IFilterRule> Filters { get; set; }
    }
}
