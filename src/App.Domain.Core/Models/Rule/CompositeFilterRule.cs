using System;
using System.Collections.Generic;
using System.Text;
using App.Domain.Core.Attributes;
using App.Domain.Core.Enums;
using App.Domain.Core.Interfaces;

namespace App.Domain.Core.Models.Rule
{
    /// <summary>
    /// Serializable filter rule structure for composites to construct deep hierarchies
    /// </summary>
    [Dto]
    public class CompositeFilterRule : IFilterRule
    {
        public List<IFilterRule> Filters { get; set; }

        public FilterCombination FilterCombination { get; set; }
    }
}
