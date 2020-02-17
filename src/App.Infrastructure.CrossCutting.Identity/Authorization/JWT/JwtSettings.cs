using System;

namespace App.Infrastructure.CrossCutting.Identity.Authorization.JWT
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        
    }
}
