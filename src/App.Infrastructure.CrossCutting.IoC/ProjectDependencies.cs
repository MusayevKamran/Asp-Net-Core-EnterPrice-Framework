using App.Application.Configurations;
using App.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.CrossCutting.IoC
{
    public static class ProjectDependencies
    {
        public static void AddProjectSetup(this IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            InjectNativeServices.RegisterServices(services);

            ApplicationDependencyConfiguration.RegisterServices(services);

            PersistenceDependencyConfiguration.RegisterServices(services);
        }
    }
}
