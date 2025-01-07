using Castle.DynamicProxy;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pienty.Diariest.Core.Contexts;
using Pienty.Diariest.Core.Helpers;
using Pienty.Diariest.Core.Middleware;
using Pienty.Diariest.Core.Services;
using Pienty.Diariest.Core.Services.Handlers;
using ServiceStack.Redis;

namespace Pienty.Diariest.API
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
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            services.AddControllers();
            services.AddHttpContextAccessor();
            
            services.AddDbContext<GeneralDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PostgreContext")));
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0.0", new OpenApiInfo { Title = "Pienty.CRM.API", Version = "v1.0.0" });
            });
            
            var redisConnectionString = Configuration.GetConnectionString("RedisContext");
            
            //redis pool
            services.AddSingleton<IRedisClientsManager>(new RedisManagerPool(redisConnectionString));
            
            //redis pool -> caching
            
            //redis contexts
            services.AddScoped<IRedisService, RedisService>();
            
            //PSQL contexts
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
            
            services.AddScoped<IAgencyService, AgencyService>();
            
            services.AddSingleton<APIMessageService>();
            
            //Helpers
            //services.AddScoped<ILogHelper, LogHelper>();

            services.AddMvc();
   
            services.AddLogging(options =>
            {
                options.AddConsole();
            });
            
            IocHelper.Init(services);
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0.0/swagger.json", "Pienty.Diariest.API");
            });

            app.UseHttpsRedirection();
            app.UseMiddleware<DiariestRequestMiddleware>();

            app.UseCors("AllowAllOrigins");
            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}