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
using App.Infrastructure.CrossCutting.Identity.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;

namespace App.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _accessor;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper, IHttpContextAccessor accessor)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _accessor = accessor;
        }


        public async Task<UserViewModel> GetByIdAsync(int id)
        {
            var entity = await _userRepository.GetByIdAsync(id);
            return _mapper.Map<UserViewModel>(entity);
        }

        public async Task<UserViewModel> GetCurrentUserAsync()
        {
            var contextIdentity = new ContextIdentity(_accessor);
            var loginId = contextIdentity.CurrentLoginId;

            var userFilter = new EntityFilter<User>(filter => filter.LoginId == loginId);
            var userSort = new EntitySort<User>();
            var entity = await _userRepository.GetAllAsync(userFilter, userSort);

            return _mapper.Map<UserViewModel>(entity.FirstOrDefault());
        }

        public async Task<IQueryable<UserViewModel>> GetListAsync()
        {
            var testModelFilter = new EntityFilter<User>();
            var testModelSort = new EntitySort<User>();

            var entityList = await _userRepository.GetAllAsync(testModelFilter, testModelSort);

            return entityList.AsQueryable().ProjectTo<UserViewModel>(_mapper.ConfigurationProvider);
        }

        public async Task<UserViewModel> InsertAsync(UserViewModel entityViewModel)
        {
            var entityMap = _mapper.Map<User>(entityViewModel);
            var entity = (User)await _userRepository.InsertAndGetAsync(entityMap);
            return _mapper.Map<UserViewModel>(entity);
        }

        public async Task UpdateAsync(UserViewModel entityViewModel)
        {
            var entity = _mapper.Map<User>(entityViewModel);

            await _userRepository.UpdateAsync(entity);
        }
    }
}
