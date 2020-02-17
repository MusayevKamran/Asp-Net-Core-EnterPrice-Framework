using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using App.Application.ViewModels.ValueObjectViewModel;

namespace App.Application.ViewModels
{
    public class UserViewModel
    {
        [Key]
        public int Id { get; set; }
        public string LoginId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? IsGenderMale { get; set; }
        public int? NativeCountryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDirector { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public LanguageViewModel Language { get; set; }
        public IconViewModel Icon { get; set; }
        public AddressViewModel Address { get; set; }
    }
}
