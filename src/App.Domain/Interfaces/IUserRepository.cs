using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using App.Domain.Interfaces.Core;
using App.Domain.Models;

namespace App.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        string Name { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();

        IQueryable<User> ListEntitiesByEmail(string name);
        IQueryable<User> ListEntitiesByPhoneNumber(string name);
    }
}
