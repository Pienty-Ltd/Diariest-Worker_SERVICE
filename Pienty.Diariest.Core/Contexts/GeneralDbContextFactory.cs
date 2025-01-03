using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pienty.Diariest.Core.Contexts
{
    public class GeneralDbContextFactory : IDesignTimeDbContextFactory<GeneralDbContext>
    {
        public GeneralDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Pienty.Diariest.API");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<GeneralDbContext>();
            var connectionString = configuration.GetConnectionString("PostgreContext");
            builder.UseNpgsql(connectionString);

            return new GeneralDbContext(builder.Options);
        }
    }
}