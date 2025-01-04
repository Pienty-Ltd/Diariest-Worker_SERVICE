using Pienty.Diariest.Core.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Pienty.Diariest.Core.Contexts
{
    
    public class GeneralDbContext : DbContext
    {
        public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options) {}
        
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasKey(model => new { model.Id });
            base.OnModelCreating(modelBuilder);
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(ToSnakeCase(entity.GetTableName()));
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToSnakeCase(property.GetColumnName()));
                }
                foreach (var key in entity.GetKeys())
                {
                    key.SetName(ToSnakeCase(key.GetName()));
                }
                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(ToSnakeCase(index.GetDatabaseName()));
                }
            }
        }

        public static string ToSnakeCase(string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }
            var startUnderscores = System.Text.RegularExpressions.Regex.Match(input, @"^_+");
            
            return startUnderscores + string.Concat(input
                    .SkipWhile(c => c == '_')
                    .Select((c, i) => i > 0 && char.IsUpper(c) ? "_" + c : c.ToString()))
                .ToLower();
        }

    }
    
}