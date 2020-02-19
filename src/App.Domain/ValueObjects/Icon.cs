using System;
using System.Collections.Generic;
using App.Domain.Core.Models;

namespace App.Domain.ValueObjects
{
    public class Icon : ValueObject
    {
        public virtual string Name { get; private set; }
        public virtual int CategoryId { get; private set; }
        public virtual string LocalizedName
        {
            get => Name;
            private set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
            }
        }

        public Icon() { }

        public Icon(string name, int categoryId, string localizedName)
        {
            Name = name;
            CategoryId = categoryId;
            LocalizedName = localizedName;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Name;
            yield return LocalizedName;
            yield return CategoryId;
        }
    }
}
