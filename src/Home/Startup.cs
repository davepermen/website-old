using Conesoft.DataSources;
using Conesoft.Users;
using Home.Services;
using Home.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace Home
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var usersPath = $"{DataSourcesImplementation.Current.SharedDirectory}/users";
            services.AddUsers("davepermen.net", usersPath);

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

            services.AddHttpClient<Conesoft.DNSimple.Client>("dnsimple");
            services.AddHttpClient<Conesoft.Ipify.Client>("ipify");
            services.AddHttpClient<Services.HttpClients.EvState.Client>("evstate");

            services.AddTransient<IScheduledTask, SimpleTicker>();
            services.AddTransient<IScheduledTask, EveryDayTicker>();
            services.AddTransient<IScheduledTask, PostFinanceMailReader>();
            services.AddTransient<IScheduledTask, FoldingAtHomeReader>();
            services.AddTransient<IScheduledTask, ServerHostingDnsUpdater>();
            services.AddTransient<IScheduledTask, GithubRepositoryReader>();
            services.AddSingleton<TickerScheduler>();

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
