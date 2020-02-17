using System;
using System.Collections.Generic;
using System.Text;
using App.Domain.Core.Attributes;
using App.Domain.Core.Interfaces;

namespace App.Domain.Core.Models.Rule
{
    /// <summary>
    ///Serializable class to use for passing sort rules around
    /// </summary>
    [Dto]
    public class SortRule : ISortRule
    {
        /// <summary>
        /// String for field name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How to sort
        /// </summary>
        public bool IsAscending { get; set; }
    }
}
