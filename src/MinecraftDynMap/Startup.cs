using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MinecraftDynMap
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseProxyRoute("mc.davepermen.net", "localhost", 8123);
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("minecraft dynmap proxy");
            });
        }
    }

    public static class ProxyMapHelper
    {
        public static IApplicationBuilder UseProxyRoute(this IApplicationBuilder builder, string domain, string host, int port = 80, string scheme = "http")
        {
            builder.UseWhen(
                _ => _.Request.Host.Host == domain,
                app => app.Use(async (context, next) =>
                {
                    context.Request.Host = new HostString(host);
                    await next.Invoke();
                }).RunProxy(new ProxyOptions()
                {
                    Host = host,
                    Port = port.ToString(),
                    Scheme = scheme
                })
            );
            return builder;
        }
    }
}
