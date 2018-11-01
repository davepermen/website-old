using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security;
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


                string secret = configuration["secret"];

                string sha1Prefix = "sha1=";

                if (context.Request.Method == "POST")
                {
                    string signature = context.Request.Headers["X-Hub-Signature"].ToString();

                    using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                    {
                        string content = await reader.ReadToEndAsync();

                        string computedSignature = Matterhook.NET.Code.Util.CalculateSignature(content, signature, secret, sha1Prefix);

                        if (computedSignature == signature)
                        {
                            using (var httpClient = new HttpClient())
                            {
                                await httpClient.GetAsync($@"http://localhost:5000/?deploymenttype=Server%20Deployment&solutionpath=C:\Users\davep\source\repos\website\");
                            }
                            await context.Response.WriteAsync("ok");
                        }
                    }
                }
                await context.Response.WriteAsync("");
            });
        }
    }
}
