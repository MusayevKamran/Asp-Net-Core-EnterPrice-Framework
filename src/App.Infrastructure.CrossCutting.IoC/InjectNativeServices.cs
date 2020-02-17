using Microsoft.Extensions.DependencyInjection;
using App.Domain.Interfaces;
using App.Infrastructure.CrossCutting.Identity.Authorization.Claims;
using App.Infrastructure.CrossCutting.Identity.ViewModels;
using App.Infrastructure.CrossCutting.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace App.Infrastructure.CrossCutting.IoC
{
    public class InjectNativeServices
    {
        public static void RegisterServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // ASP.NET Authorization Polices
            services.AddSingleton<IAuthorizationHandler, ClaimsRequirementHandler>();


        }
    }
}
