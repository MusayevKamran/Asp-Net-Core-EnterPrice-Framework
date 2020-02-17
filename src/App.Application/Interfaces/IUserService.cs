using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Application.ViewModels;

namespace App.Application.Interfaces
{
    public interface IUserService
    {
        Task<IQueryable<UserViewModel>> GetListAsync();
        Task<UserViewModel> GetByIdAsync(int id);
        Task UpdateAsync(UserViewModel entity);
        Task<UserViewModel> InsertAsync(UserViewModel entity);
    }
}
