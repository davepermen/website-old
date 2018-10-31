using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace UpdateFromGithub
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            IApplicationLifetime applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            IConfiguration configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

            app.Run(async (context) =>
            {
                if (context.Request.Method == "POST"
                    && configuration["X-GitHub-Event"] == context.Request.Headers["X-GitHub-Event"]
                    && configuration["X-Hub-Signature"] == context.Request.Headers["X-Hub-Signature"])
                {
                    File.WriteAllText("request.txt", "update now");
                    await context.Response.WriteAsync("ok");
                    //applicationLifetime.StopApplication();
                }
                await context.Response.WriteAsync("");
            });
        }
    }
}
