using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Serilog.Sinks.AWSDynamoDb.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var currentEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{currentEnv}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.AwsDynamoDb("{AWS-SERVICE-REGION}", "{AWS-ACCESS-KEY}", "{AWS-SECRET KEY}", "Logs", autoCreateTable: true)
                .CreateLogger();

            // Log information
            Log.Information("Starting Serilog.Sinks.AWSDynamoDb.Sample");

            // Log exception
            Log.Error(new Exception("This is an exception"), "Logging an exception");

            // Log a class
            Log.Information("Logging a class {@Hero}", Hero.ReturnDummyHero());

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
    }
}
