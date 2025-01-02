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
            modelBuilder.Entity<User>().HasKey(model => new { model.Id });
        }

    }
    
}