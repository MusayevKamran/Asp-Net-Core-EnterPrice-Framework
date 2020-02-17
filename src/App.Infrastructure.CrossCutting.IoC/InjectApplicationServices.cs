using App.Application.Interfaces;
using App.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.CrossCutting.IoC
{
    public class InjectApplicationServices
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

        }
    }
}
