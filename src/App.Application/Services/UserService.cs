using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Application.Interfaces;
using App.Application.ViewModels;
using App.Domain.Core.Models;
using App.Domain.Interfaces;
using App.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;

namespace App.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor accessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _accessor = accessor;
        }

        public string Name => GetName();

        public string CurrentLoginId => GetCurrentIdentity();

        private string GetName()
        {
            return _accessor.HttpContext.User.Identity.Name ??
                   _accessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }

        private string GetCurrentIdentity()
        {
            return _accessor.HttpContext.User.Claims.FirstOrDefault()?.Value;
        }

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }

        public async Task<UserViewModel> GetByIdAsync(int id)
        {
            var entity = await _userRepository.GetEntityByIdAsync(id);
            return _mapper.Map<UserViewModel>(entity);
        }

        public async Task<UserViewModel> GetCurrentUserAsync()
        { 
            
            if (CurrentLoginId == null)
            {
                throw new Exception("User is not authorized");
            }
            var userFilter = new EntityFilter<User>(filter => filter.LoginId == CurrentLoginId);
            var userSort = new EntitySort<User>();
            var entity = await _userRepository.ListEntitiesAsync(userFilter, userSort);

            return _mapper.Map<UserViewModel>(entity.FirstOrDefault());
        }

        public async Task<IQueryable<UserViewModel>> GetListAsync()
        {
            var testModelFilter = new EntityFilter<User>();
            var testModelSort = new EntitySort<User>();

            var entityList = await _userRepository.ListEntitiesAsync(testModelFilter, testModelSort);

            return entityList.AsQueryable().ProjectTo<UserViewModel>(_mapper.ConfigurationProvider);
        }

        public async Task<UserViewModel> InsertAsync(UserViewModel entityViewModel)
        {
            var entityMap = _mapper.Map<User>(entityViewModel);
            var entity = (User)await _userRepository.InsertEntityAsync(entityMap);
            return _mapper.Map<UserViewModel>(entity);
        }

        public async Task UpdateAsync(UserViewModel entityViewModel)
        {
            var entity = _mapper.Map<User>(entityViewModel);

            await _userRepository.UpdateEntityAsync(entity);
        }
    }
}
