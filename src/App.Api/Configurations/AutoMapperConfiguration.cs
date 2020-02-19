using System;
using App.Application.AutoMapper;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace App.Api.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAutoMapper(typeof(DomainToViewModelMappingProfile), typeof(ViewModelToDomainMappingProfile));
        }
    }
}