using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using App.Domain.Interfaces.Core;
using App.Domain.Models;

namespace App.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        IQueryable<User> ListEntitiesByEmail(string name);
        IQueryable<User> ListEntitiesByPhoneNumber(string name);
    }
}
