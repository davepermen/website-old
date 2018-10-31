using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace UpdateFromGithub
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            IApplicationLifetime applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            app.Run(async (context) =>
            {
                if (context.Request.Method == "POST")
                {
                    File.WriteAllText("request.txt", await new StreamReader(context.Request.Body).ReadToEndAsync());
                    await context.Response.WriteAsync(await new StreamReader(context.Request.Body).ReadToEndAsync());
                    //applicationLifetime.StopApplication();
                }
                await context.Response.WriteAsync("");
            });
        }
    }
}
