﻿using Conesoft;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace EvState
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .AddDataSourceConfiguration()
                .UseStartup<Startup>();
    }
}