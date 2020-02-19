using App.Domain.Core.Interfaces;
using App.Domain.Core.Models;
using App.Domain.Interfaces;
using App.Domain.Interfaces.Core;
using App.Infrastructure.CrossCutting.Identity.Context;
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
            // Infra - Persistence
            services.AddScoped<IRepositoryBase, RepositoryBase>();
            services.AddScoped<IRepository, Repository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            services.AddScoped<IUserRepository, UserRepository>();



            services.AddDbContext<AppDbContext>();
            services.AddDbContext<ApplicationDbContext>();
        }
    }
}
