using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pienty.Diariest.Core.Contexts;
using Pienty.Diariest.Core.Helpers;
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
                c.SwaggerDoc("v1.0.0", new OpenApiInfo { Title = "Pienty.CRM API", Version = "v1.0.0" });
            });
            
            var redisConnectionString = Configuration.GetConnectionString("RedisContext");
            
            //redis pool
            services.AddSingleton<IRedisClientsManager>(new RedisManagerPool(redisConnectionString));
            
            //redis contexts
            services.AddScoped<IRedisService, RedisService>();
            
            //PSQL contexts
            services.AddScoped<IDbService, DbService>();
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IUserService, UserService>();
            /*services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IErrorLoggerService, ErrorLoggerService>();
            services.AddScoped<ILoginLoggerService, LoginLoggerService>();*/
            
            //Helpers
            //services.AddScoped<ILogHelper, LogHelper>();
            
            //Other Services
            //services.AddSingleton<APIMessageService>();

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