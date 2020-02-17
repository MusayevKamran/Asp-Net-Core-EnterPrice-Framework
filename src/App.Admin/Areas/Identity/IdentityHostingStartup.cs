using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(App.Admin.Areas.Identity.IdentityHostingStartup))]
namespace App.Admin.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}