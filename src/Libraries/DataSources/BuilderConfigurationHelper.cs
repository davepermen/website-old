using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Conesoft
{
    public static class BuilderConfigurationHelper
    {
        static public IWebHostBuilder AddDataSourceConfiguration(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile(DataSources.Configuration, optional: true, reloadOnChange: true);
            });
        }
    }
}
