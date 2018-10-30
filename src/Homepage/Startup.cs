using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Homepage
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseProxyRoute("minecraft.davepermen.net", "localhost", 8123);
            //app.UseProxyRoute("charging.davepermen.net", "localhost", 65523);
            //app.UseProxyRoute("pushups.davepermen.net", "localhost", 65524);

            if (Directory.Exists(@"C:\Users\davep\Documents\Files"))
            {
                var options = new FileServerOptions
                {
                    FileProvider = new PhysicalFileProvider(@"C:\Users\davep\Documents\Files"),
                    RequestPath = "/files",
                    EnableDirectoryBrowsing = true
                };
                options.StaticFileOptions.ServeUnknownFileTypes = true;
                options.StaticFileOptions.DefaultContentType = "application/octet-stream";
                app.UseFileServer(options);
            }

            app.UseDefaultFiles();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".rdp"] = "application/x-rdp";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
            });

            app.UseMvc();

            // app.Run(async (context) =>
            // {
            //     await context.Response.WriteAsync("Hello World!");
            // });
        }
    }

    //public class PushController : Controller
    //{
    //    [HttpPost("/github-update")]
    //    public IActionResult HandleGithubChange([FromBody]object message)
    //    {
    //        System.IO.File.WriteAllText("github-update.json", message.ToString());

    //        return new OkResult();
    //    }
    //}

    //public static class ProxyMapHelper
    //{
    //    public static IApplicationBuilder UseProxyRoute(this IApplicationBuilder builder, string domain, string host, int port = 80, string scheme = "http")
    //    {
    //        builder.UseWhen(
    //            _ => _.Request.Host.Host == domain,
    //            app => app.Use(async (context, next) =>
    //            {
    //                context.Request.Host = new HostString(host);
    //                await next.Invoke();
    //            }).RunProxy(new ProxyOptions()
    //            {
    //                Host = host,
    //                Port = port.ToString(),
    //                Scheme = scheme
    //            })
    //        );
    //        return builder;
    //    }
    //}
}
