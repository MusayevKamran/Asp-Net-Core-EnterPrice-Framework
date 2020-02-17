using System;
using System.Collections.Generic;
using System.Text;
using App.Domain.Core.Models;

namespace App.Domain.ValueObjects
{
    public class Language : ValueObject
    {
        public virtual string Name { get; private set; }
        public virtual string LocalizedName { get; private set; }
        public virtual string Description { get; private set; }
        public virtual string IsoCode { get; private set; }


        private Language() { }

        public Language(string name, string localizedName, string description, string isoCode)
        {
            Name = name;
            LocalizedName = localizedName;
            Description = description;
            IsoCode = isoCode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return LocalizedName;
            yield return Description;
            yield return IsoCode;
        }

    }
}
