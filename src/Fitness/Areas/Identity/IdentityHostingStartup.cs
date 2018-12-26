using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(User_Experiment.Areas.Identity.IdentityHostingStartup))]
namespace User_Experiment.Areas.Identity
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