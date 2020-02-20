using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.CrossCutting.Identity.Models
{
    public class ContextIdentity
    {
        private static IHttpContextAccessor _accessor;

        public ContextIdentity(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        
        public string CurrentLoginId => GetCurrentIdentity();
        private string GetCurrentIdentity()
        {
            if (_accessor?.HttpContext?.User?.Claims?.FirstOrDefault()?.Value == null)
            {
                throw new Exception("User is not authorized");
            }

            return _accessor.HttpContext.User.Claims.FirstOrDefault()?.Value;
        }

        public string Name => GetName();
        private string GetName()
        {
            return _accessor.HttpContext.User.Identity.Name ??
                   _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        public bool IsAuthenticated => CheckUserIsAuthenticated();
        private bool CheckUserIsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }
    }
}
