using System;
using System.Collections.Generic;
using System.Text;
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
            throw new NotImplementedException();
        }
    }
}
