﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace RemoteApplications
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile($@"{Directory.GetCurrentDirectory()}\..\{typeof(Program).Namespace}.json", optional: true, reloadOnChange: true);
                })
                .UseStartup<Startup>();

        public static string DataRoot = Directory.Exists($@"{Directory.GetCurrentDirectory()}\..\Data\{typeof(Program).Namespace}")
            ? $@"{Directory.GetCurrentDirectory()}\..\Data\{typeof(Program).Namespace}"
            : $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Webseiten\{typeof(Program).Namespace}"
            ;
    }
}