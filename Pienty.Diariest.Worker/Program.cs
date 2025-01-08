using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Configurations;
using Pienty.Diariest.Worker.Workers;

namespace Pienty.Diariest.Worker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            var provider = host.Services;

            provider.UseScheduler(scheduler =>
            {
                scheduler.Schedule<GeneralWorker>().EverySeconds(5).RunOnceAtStart();
                scheduler.Schedule<CacheableWorker>().Hourly().RunOnceAtStart();
            }).LogScheduledTaskProgress(provider.GetService<ILogger<IScheduler>>());

            await host.RunAsync();
        }
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    int minThreads = 0;
                    int minIOThreads = 10;

                    ThreadPool.GetMinThreads(out minThreads, out minIOThreads);
                    minThreads = 2048;

                    ThreadPool.SetMinThreads(minThreads, minIOThreads);
                    
                    
                    var builder = new ConfigurationBuilder()
                        .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var configuration = builder.Build();
                    
                    services.Configure<ApplicationConfig>(configuration.GetSection(ApplicationConfig.KeyName));
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}