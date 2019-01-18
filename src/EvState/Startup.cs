using Conesoft;
using EvState.HttpClients;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace EvState
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataSources();

            services.AddHttpClient<ECarUpHttpClient>();
            services.AddHttpClient<EVNotifyHttpClient>();

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Expiration = TimeSpan.FromDays(365);
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.SlidingExpiration = true;
                options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo($"{new DataSources().SharedDirectory}/keys"));
            });

            services.AddMvc()
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Livetile", "/livetile.xml");
            });

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
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
