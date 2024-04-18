using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RazorPages.Models;

namespace RazorPages.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("person");
            builder.Property(x => x.Id).HasColumnName("person_id");
            builder.Property(x => x.Name).HasColumnName("person_name");
            builder.HasKey(x => x.Id);
        }
    }
}
