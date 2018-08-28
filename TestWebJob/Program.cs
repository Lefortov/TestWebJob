using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestWebJob.CQRS.Commands;
using TestWebJob.CQRS.Queries;
using TestWebJob.Models;
using TestWebJob.Utils;

namespace TestWebJob
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; }
        
        static void Main(string[] args)
        {
            // .NET Core sets the source directory as the working directory - so change that:
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var configuration = new JobHostConfiguration();
            configuration.Queues.MaxPollingInterval = TimeSpan.FromSeconds(10);
            configuration.Queues.VisibilityTimeout = TimeSpan.FromMinutes(1);
            configuration.Queues.BatchSize = 1;
            configuration.JobActivator = new CustomJobActivator(serviceCollection.BuildServiceProvider());
            configuration.UseTimers();
            
            var host = new JobHost(configuration);
            
            host.RunAndBlock();
        }
        
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var configuration = GetWebJobConfiguration();

            // DI-Configuration
            serviceCollection.AddTransient<AddPriceRecordCommand, AddPriceRecordCommand>();
            serviceCollection.AddTransient<FuelPricesQuery, FuelPricesQuery>();
            
            serviceCollection.Configure<AppSettings>(settings =>
            {
                settings.Filters = new Filters()
                {
                    TakenDays = Convert.ToInt32(Configuration["Filters:TakenDays"])
                };
            });
            
            serviceCollection.AddDbContext<WebJobContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:SqlServer"]));
            
            AddWebJobsCommonServices(configuration);
        }

        private static void AddWebJobsCommonServices(IConfigurationRoot configuration)
        {
            if (String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("AzureWebJobsStorage")))
            {
                // Env variables would be set on azure. But not locally. If missing, set them to the connection string
                Environment.SetEnvironmentVariable("AzureWebJobsStorage", configuration.GetConnectionString("AzureWebJobsStorage"));
                Environment.SetEnvironmentVariable("AzureWebJobsDashboard", configuration.GetConnectionString("AzureWebJobsDashboard"));
            }
        }

        private static IConfigurationRoot GetWebJobConfiguration()
        {   
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}