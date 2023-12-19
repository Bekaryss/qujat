using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System;
using Microsoft.AspNetCore.Hosting;
using Qujat.Core.Extensions;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Qujat.Backoffice.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var app = builder.Build();

            var services = app.Services;
            //services.ExecuteMigrations();
           
            //await services.SeedDefaultIcons();


            Console.WriteLine("...");

            await app.RunAsync();
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine(environment);

            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3} {XCorrelationId}] - {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .Enrich.FromLogContext()
                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", optional: false)
                .Build();

            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(configuration);
                    webBuilder.UseStartup<Startup>();
                });

            hostBuilder.UseSerilog();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            return hostBuilder;
        }
    }
}
