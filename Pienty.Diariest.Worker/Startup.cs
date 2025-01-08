using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Extensions;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Worker.Workers;
using ServiceStack.Redis;

namespace Pienty.Diariest.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            
            var redisConnectionString = Configuration.GetConnectionString("RedisContext");
            
            //redis pool
            services.AddSingleton<IRedisClientsManager>(new RedisManagerPool(redisConnectionString));

            //all extensions
            services.AddDatabase();
            services.AddCacheableService();
            //services.AddGenerativeAI();

            //Workers
            services.AddTransient<GeneralWorker>();
            services.AddTransient<CacheableWorker>();

            services.AddLogging(options =>
            {
                options.AddConsole();
            });
            
            services.AddScheduler();
            
            IocHelper.Init(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();
        }
    }
}