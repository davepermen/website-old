using Microsoft.Extensions.DependencyInjection;

namespace Conesoft
{
    public static class ServicesConfigurationHelper
    {
        static public IServiceCollection AddDataSources(this IServiceCollection serviceCollection) => serviceCollection.AddSingleton<IDataSources, DataSources>();
    }
}
