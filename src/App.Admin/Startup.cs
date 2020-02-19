using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using App.Admin.Configurations;
using App.Infrastructure.CrossCutting.Identity.Configurations;
using App.Infrastructure.CrossCutting.IoC;
using App.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App.Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Setting DBContexts
            services.AddAppDatabaseSetup(Configuration);
            services.AddApplicationDatabaseSetup(Configuration);

            // ASP.NET Identity Settings
            services.AddIdentitySetup();

            // AutoMapper Settings
            services.AddAutoMapperSetup();

            // MVC Settings
            services.AddControllersWithViews();


            services.AddRazorPages();

            // Authentication & Authorization
            services.AddAuthSetup(Configuration);

            // .NET Native DI Abstraction
            services.AddProjectSetup();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
