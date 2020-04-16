using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conesoft.DataSources
{
    public static class BuilderConfigurationHelper
    {
        static public IWebHostBuilder AddDataSourceConfiguration(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddJsonFile(DataSourcesImplementation.Configuration, optional: true, reloadOnChange: true);
            }).ConfigureServices(services =>
            {
                services.AddSingleton(DataSourcesImplementation.Current);
            });
        }
    }
}
