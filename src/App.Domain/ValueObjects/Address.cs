using System.Collections.Generic;
using App.Domain.Core.Models;

namespace App.Domain.ValueObjects
{
    public class Address : ValueObject
    {
        public virtual string Street { get; private set; }
        public virtual string City { get; private set; }
        public virtual string State { get; private set; }
        public virtual string Country { get; private set; }
        public virtual string ZipCode { get; private set; }

        private Address() { }

        public Address(string street, string city, string state, string country, string zipcode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipcode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
        }
    }
}
