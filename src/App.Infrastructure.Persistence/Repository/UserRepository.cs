using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using App.Domain.Interfaces;
using App.Domain.Interfaces.Core;
using App.Domain.Models;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Repository.Core;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.Persistence.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IRepository _repository;

        public UserRepository(AppDbContext context, IRepository repository)
            : base(context, repository)
        {
            _repository = repository;
        }

        public UserRepository(IHttpContextAccessor accessor, AppDbContext context, IRepository repository)
            : base(context, repository)
        {
            _accessor = accessor;
            _repository = repository;
        }


        public string Name => GetName();

        private string GetName()
        {
            return _accessor.HttpContext.User.Identity.Name ??
                   _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public IQueryable<User> ListEntitiesByEmail(string email) =>
            _repository.GetListCore<User>(filter => filter.Email == email);

        public IQueryable<User> ListEntitiesByPhoneNumber(string phoneNumber) =>
            _repository.GetListCore<User>(filter => filter.PhoneNumber == phoneNumber);
    }
}
