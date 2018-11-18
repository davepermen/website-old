using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace UpdateFromGithub
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            var fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            fileExtensionContentTypeProvider.Mappings.Clear();
            fileExtensionContentTypeProvider.Mappings[".application"] = "application/x-ms-application";
            fileExtensionContentTypeProvider.Mappings[".manifest"] = "application/x-ms-manifest";
            fileExtensionContentTypeProvider.Mappings[".deploy"] = "application/octet-stream";
            fileExtensionContentTypeProvider.Mappings[".exe"] = "image/png";

            var physicalFileProvider = new PhysicalFileProvider($@"{Directory.GetCurrentDirectory()}\..\data\{typeof(Program).Namespace}\ContinuousDeployment");

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = fileExtensionContentTypeProvider,
                FileProvider = physicalFileProvider,
                RequestPath = "/cd"
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

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
