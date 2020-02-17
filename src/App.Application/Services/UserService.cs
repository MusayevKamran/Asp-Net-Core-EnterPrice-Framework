using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Application.Interfaces;
using App.Application.ViewModels;
using App.Domain.Core.Models;
using App.Domain.Interfaces;
using App.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace App.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserViewModel> GetByIdAsync(int id)
        {
            var entity = await _userRepository.GetEntityByIdAsync(id);
            return _mapper.Map<UserViewModel>(entity);
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
