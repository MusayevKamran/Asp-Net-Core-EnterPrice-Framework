using System;
using System.Collections.Generic;
using System.Text;

namespace App.Infrastructure.CrossCutting.Identity.Models
{
    public class AuthenticationResult
    {
        public string LoginId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public double? ExpiresIn { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}