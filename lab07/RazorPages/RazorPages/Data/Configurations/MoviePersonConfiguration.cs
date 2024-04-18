using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RazorPages.Models;

namespace RazorPages.Data.Configurations
{
    public class MoviePersonConfiguration : IEntityTypeConfiguration<MoviePerson>
    {
        public void Configure(EntityTypeBuilder<MoviePerson> builder)
        {
            builder.ToTable("movie_cast");
            builder.Property(x => x.MovieId).HasColumnName("movie_id");
            builder.Property(x => x.PersonId).HasColumnName("person_id");
            builder.Property(x => x.CharacterName).HasColumnName("character_name");
        }
    }
}
