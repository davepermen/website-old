using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text;

namespace UpdateFromGithub
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Run(async (context) =>
            {
                var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

                var secret = configuration["secret"];

                var sha1Prefix = "sha1=";

                if (context.Request.Method == "POST")
                {
                    var signature = context.Request.Headers["X-Hub-Signature"].ToString();

                    using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        var content = await reader.ReadToEndAsync();

                        var computedSignature = Matterhook.NET.Code.Util.CalculateSignature(content, signature, secret, sha1Prefix);

                        if (computedSignature == signature)
                        {
                            var batchPath = Path.Combine(Path.GetTempPath(), "build.cmd");
                            File.WriteAllText(batchPath,
                            $@"
                            cd {configuration["project-directory"]}
                            FOR /D %%A IN (*) DO (
                                cd %%A
                                dotnet publish -p:PublishDir={configuration["target-directory"]}%%A
                                cd ..
                            )");
                            System.Diagnostics.Process.Start(batchPath).WaitForExit();
                            File.WriteAllText("request.txt", $"updated from github");
                            await context.Response.WriteAsync("ok");
                        }
                    }
                }
                await context.Response.WriteAsync("");
            });
        }
    }
}
