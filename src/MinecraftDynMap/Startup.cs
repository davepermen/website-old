using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MinecraftDynMap
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) => app.RunProxy(new ProxyOptions()
        {
            Host = "localhost",
            Port = 8123.ToString(),
            Scheme = "http"
        });
    }
}
