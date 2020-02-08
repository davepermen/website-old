using Conesoft;
using Conesoft.Users;
using EvState.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace EvState
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataSources();

            services.AddHttpClient<ECarUpHttpClient>();

            services.AddSingleton<ScheduledTasks.EvState>();
            services.AddSingleton<Services.IScheduledTask, ScheduledTasks.PollEvState>();
            // needs to be removed asap
            services.AddSingleton(s => s.GetServices<Services.IScheduledTask>().OfType<ScheduledTasks.PollEvState>().First());
            services.AddSingleton<Services.TaskScheduler>();

            services.AddUsers(s => new UsersRootPath($"{s.GetService<IDataSources>().SharedDirectory}/users")); 

            services.AddServerSideBlazor();
            services.AddRazorPages(options =>
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseUsers();

            app.UseRouting();

            app.ApplicationServices.GetService<Services.TaskScheduler>().Start();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
