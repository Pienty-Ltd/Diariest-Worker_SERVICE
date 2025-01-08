using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Pienty.Diariest.Core.Middleware;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;

namespace Pienty.Diariest.Core.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddGenerativeAI(this IServiceCollection services)
        {
            services.AddScoped<IAIService, AIService>();

            return services;
        }
        
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            //redis contexts
            services.AddScoped<IRedisService, RedisService>();
            
            //redis pool -> db caching to memory
            services.AddScoped<DbCachingInterceptor>();
            
            //sql
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IDbService, DbService>();

            services.AddScoped<IPageService, PageService>();

            services.AddScoped<UserService>();
            services.AddScoped<IUserService>(provider =>
            {
                var proxyGenerator = new ProxyGenerator();
                var service = provider.GetService<UserService>();
                if (service == null)
                {
                    throw new InvalidOperationException("UserService is not registered in the service provider.");
                }
                
                var cachingInterceptor = provider.GetService<DbCachingInterceptor>();

                return proxyGenerator.CreateInterfaceProxyWithTarget<IUserService>(service, cachingInterceptor);
            });

            services.AddScoped<AgencyService>();
            services.AddScoped<IAgencyService>(provider =>
            {
                var proxyGenerator = new ProxyGenerator();
                var service = provider.GetService<AgencyService>();
                if (service == null)
                {
                    throw new InvalidOperationException("AgencyService is not registered in the service provider.");
                }
                
                var cachingInterceptor = provider.GetService<DbCachingInterceptor>();

                return proxyGenerator.CreateInterfaceProxyWithTarget<IAgencyService>(service, cachingInterceptor);
            });

            return services;
        }
    }
}