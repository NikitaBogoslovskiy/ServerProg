using Microsoft.EntityFrameworkCore;
using RazorPages.Models;

namespace RazorPages.Data
{
    public static class ModelBuilderExtension
    {
        public static ModelBuilder Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Testimonial>().HasData(
                new Testimonial
                {
                    Id = 1,
                    Name = "James Fernando",
                    Occupation = "Manager of Racer",
                    ImgUrl = "uploads/testi_01.png",
                    Title = "Wonderful Support!",
                    Description = "They have got my project on time with the competition with a sed highly skilled, and experienced & professional team."
                },
                new Testimonial
                {
                    Id = 2,
                    Name = "Jacques Philips",
                    Occupation = "Designer",
                    ImgUrl = "uploads/testi_02.png",
                    Title = "Awesome Services!",
                    Description = "Explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you completed."
                },
                new Testimonial
                {
                    Id = 3,
                    Name = "Venanda Mercy",
                    Occupation = "Newyork City",
                    ImgUrl = "uploads/testi_03.png",
                    Title = "Great & Talented Team!",
                    Description = "The master-builder of human happines no one rejects, dislikes avoids pleasure itself, because it is very pursue pleasure."
                }
            );
            return modelBuilder;
        }
    }
}
