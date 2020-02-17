using App.Domain.Core.Interfaces;
using App.Domain.Core.Models;
using App.Domain.Interfaces;
using App.Domain.Interfaces.Core;
using App.Infrastructure.CrossCutting.Identity;
using App.Infrastructure.CrossCutting.Identity.Authorization.JWT;
using App.Infrastructure.CrossCutting.Identity.Context;
using App.Infrastructure.CrossCutting.Identity.Interfaces;
using App.Infrastructure.CrossCutting.Identity.Services;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Repository;
using App.Infrastructure.Persistence.Repository.Core;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.CrossCutting.IoC
{
    public class InjectInfrastructureServices
    {

        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IRepositoryBase, RepositoryBase>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Infra - Identity
            services.AddScoped<ILoginManager, LoginManager>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            // Infra - Persistence
            services.AddScoped<IUserRepository, UserRepository>();



            services.AddDbContext<AppDbContext>();
            services.AddDbContext<ApplicationDbContext>();
        }
    }
}
