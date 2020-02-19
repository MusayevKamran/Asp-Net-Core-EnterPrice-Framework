using Microsoft.Extensions.DependencyInjection;
using App.Infrastructure.CrossCutting.Identity.Authorization.Claims;
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
