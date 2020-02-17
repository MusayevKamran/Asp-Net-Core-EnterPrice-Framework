using System.Collections.Generic;

namespace App.Infrastructure.CrossCutting.Identity.ViewModels.OutputModels
{
    public class AuthFailedOutputModel
    {
        public IEnumerable<string> Errors { get; set; }
    }
}