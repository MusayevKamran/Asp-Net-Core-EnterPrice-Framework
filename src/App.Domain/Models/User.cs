using System;
using App.Domain.Core.Models;
using App.Domain.ValueObjects;

namespace App.Domain.Models
{
    public class User : EntityBase
    {
        public virtual string LoginId { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual string Email { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual int? IsGenderMale { get; set; }
        public virtual int? NativeCountryId { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsDirector { get; set; }
        public virtual DateTime? DateCreated { get; set; }
        public virtual DateTime? DateModified { get; set; }
        public virtual Language Language { get; set; }
        public virtual Icon Icon { get; set; }
        public virtual Address Address { get; set; }
    }
}
