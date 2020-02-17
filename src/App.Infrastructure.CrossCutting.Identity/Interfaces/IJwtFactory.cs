using System.Security.Claims;
using System.Threading.Tasks;
using App.Infrastructure.CrossCutting.Identity.Models;
using App.Infrastructure.CrossCutting.Identity.ViewModels;

namespace App.Infrastructure.CrossCutting.Identity.Interfaces
{
    public interface IJwtFactory
    {
        Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(Login login);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
