using App.Domain.Core.Interfaces;
using App.Domain.Core.Models;
using App.Domain.Interfaces;
using App.Domain.Interfaces.Core;
using App.Infrastructure.Persistence.Repository;
using App.Infrastructure.Persistence.Repository.Core;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.Persistence.Configurations
{
    public class PersistenceDependencyConfiguration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Infra - Persistence
            services.AddScoped<IRepositoryBase, RepositoryBase>();
            services.AddScoped<IRepository, Repository.Core.Repository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            services.AddScoped<IUserRepository, UserRepository>();

        }
    }
}
