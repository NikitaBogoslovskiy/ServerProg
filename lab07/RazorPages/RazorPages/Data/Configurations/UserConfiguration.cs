using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RazorPages.Models;

namespace RazorPages.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("user");
            builder.Property(x => x.Login).HasColumnName("login");
            builder.Property(x => x.Password).HasColumnName("password");
            builder.Property(x => x.Role).HasColumnName("role");
        }
    }
}
