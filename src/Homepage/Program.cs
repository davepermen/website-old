using Conesoft;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Homepage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.AddDataSourceConfiguration();
                    webBuilder.UseStartup<Startup>();
                });
    }
}
