using System;
using App.Infrastructure.CrossCutting.Identity.Context;
using App.Infrastructure.CrossCutting.Identity.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Admin.Configurations
{
    public static class IdentityConfiguration
    {
        public static void AddIdentitySetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDefaultIdentity<Login>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        public static void AddAuthSetup(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAuthentication()
                .AddFacebook(o =>
                {
                    o.AppId = configuration["Authentication:Facebook:AppId"];
                    o.AppSecret = configuration["Authentication:Facebook:AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                });
        }
    }
}