using Microsoft.EntityFrameworkCore;
using RazorPages.Data.Configurations;
using RazorPages.Models;

namespace RazorPages.Data
{
    public class MoviesContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data source=movies.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
        }
    }
}
