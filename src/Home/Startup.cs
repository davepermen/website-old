using Conesoft;
using Conesoft.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Compression;
using System.Linq;

namespace Home
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataSources();
            services.AddUsers(s => $"{s.GetService<IDataSources>().SharedDirectory}/users");

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "image/jpeg", "image/png", "application/font-woff2", "image/svg+xml"
                });
                options.EnableForHttps = true;
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseResponseCompression();

            app.UseUsers();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
