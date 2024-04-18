using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RazorPages.Models;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace RazorPages.Data.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("movie");
            builder.Property(x => x.Id).HasColumnName("movie_id").ValueGeneratedOnAdd();
            builder.Property(x => x.Title).HasColumnName("title");
            builder.Property(x => x.Budget).HasColumnName("budget");
            builder.Property(x => x.Homepage).HasColumnName("homepage");
            builder.Property(x => x.Overview).HasColumnName("overview");
            builder.Property(x => x.Popularity).HasColumnName("popularity");
            builder.Property(x => x.ReleaseDate).HasColumnName("release_date");
            builder.Property(x => x.Revenue).HasColumnName("revenue");
            builder.Property(x => x.Runtime).HasColumnName("runtime");
            builder.Property(x => x.MovieStatus).HasColumnName("movie_status");
            builder.Property(x => x.Tagline).HasColumnName("tagline");
            builder.Property(x => x.VoteAverage).HasColumnName("vote_average");
            builder.Property(x => x.VoteCount).HasColumnName("vote_count");
            builder.HasKey(x => x.Id);
        }
    }
}
