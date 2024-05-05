using Microsoft.EntityFrameworkCore;
using RazorPages.Data.Configurations;
using RazorPages.Models;

namespace RazorPages.Data
{
    public class MoviesContext : DbContext
    {
        public DbSet<MoviePerson> MoviePersons { get; set; } = default!;
        public DbSet<Movie> Movies { get; set; } = default!;
        public DbSet<Person> Persons { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data source=movies.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MoviePersonConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            modelBuilder.Entity<Movie>()
                .HasMany(e => e.Persons)
                .WithMany(e => e.Movies)
                .UsingEntity<MoviePerson>();
        }
    }
}
