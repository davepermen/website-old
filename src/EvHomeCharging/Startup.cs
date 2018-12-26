using Conesoft;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace EvHomeCharging
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataSources();

            services.AddTransient<Charges>();

            var username = configuration["ecarup-username"];
            var password = configuration["ecarup-password"];
            services.AddHttpClient("ECarUp", client =>
            {
                client.BaseAddress = new Uri("https://www.ecarup.com/");
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password)));
            });
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
            provider.Mappings[".webmanifest"] = "text/json";
            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = provider
            });


            app.UseMvcWithDefaultRoute();
        }
    }
}
