using Castle.DynamicProxy;
using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Middleware;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;
using Pienty.Diariest.Worker.Workers;

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

            services.AddScoped<IDbService, DbService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<DbCachingInterceptor>();
            services.AddScoped<UserService>();
            services.AddScoped<IUserService>(provider =>
            {
                var proxyGenerator = new ProxyGenerator();
                var userService = provider.GetService<UserService>();
                if (userService == null)
                {
                    throw new InvalidOperationException("UserService is not registered in the service provider.");
                }
                
                var cachingInterceptor = provider.GetService<DbCachingInterceptor>();

                return proxyGenerator.CreateInterfaceProxyWithTarget<IUserService>(userService, cachingInterceptor);
            });

            services.AddTransient<GeneralWorker>();
            /*services.AddScoped<IMailService, MailService>();
            
            services.AddSingleton<ISimdiRequest, SimdiRequest>();
            services.AddSingleton<ISmsRequest, SmsRequest>();

            services.AddTransient<WorkAreaWorker>();
            services.AddTransient<GeneralWorker>();
            services.AddTransient<SmsWorker>();
            services.AddTransient<MailWorker>();
            services.AddTransient<PushWorker>();*/

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