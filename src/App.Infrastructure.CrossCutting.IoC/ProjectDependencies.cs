using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.CrossCutting.IoC
{
    public static class ProjectDependencies
    {
        public static void AddProjectSetup(this IServiceCollection services)
        {
            // Adding dependencies from another layers (isolated from Presentation)
            InjectNativeServices.RegisterServices(services);
            InjectInfrastructureServices.RegisterServices(services);
            InjectApplicationServices.RegisterServices(services);
        }
    }
}
