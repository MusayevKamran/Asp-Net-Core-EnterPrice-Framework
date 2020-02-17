using System;
using System.Collections.Generic;
using System.Text;

namespace App.Application.ViewModels.ValueObjectViewModel
{
    public class LanguageViewModel
    {
        public string Name { get; set; }
        public string LocalizedName { get; set; }
        public string Description { get; set; }
        public string IsoCode { get; set; }
    }
}
