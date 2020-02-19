using System;

namespace App.Domain.Core.Attributes
{
    /// <summary>
    /// Specifies attribute to know which objects to add to known types automatically,
    /// besides the types derived from entity base, operation base and IDto
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct,
        AllowMultiple = false, Inherited = true)]
    public class DtoAttribute : Attribute
    {
    }
}
