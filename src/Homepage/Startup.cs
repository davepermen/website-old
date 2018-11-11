using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Homepage
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDirectoryBrowser();
            services.AddMvc();

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

            app.UseDefaultFiles();

            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings[".rdp"] = "application/x-rdp";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider,
            });

            app.UseMvc();
        }
    }
}
