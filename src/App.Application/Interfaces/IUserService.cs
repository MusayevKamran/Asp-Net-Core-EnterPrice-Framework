using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Application.ViewModels;

namespace App.Application.Interfaces
{
    public interface IUserService
    {
        Task<IQueryable<UserViewModel>> GetListAsync();
        Task<UserViewModel> GetByIdAsync(int id);
        Task<UserViewModel> GetCurrentUserAsync();
        Task UpdateAsync(UserViewModel entity);
        Task<UserViewModel> InsertAsync(UserViewModel entity);
    }
}
