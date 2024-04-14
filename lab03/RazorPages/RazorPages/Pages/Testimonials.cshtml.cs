using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPages.Data;
using RazorPages.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace RazorPages.Pages
{
    [Bind]
    public class TestimonialForm
    {
        [BindProperty]
        [Required]
        public string? Name { get; set; }

        [BindProperty]
        [Required]
        public string? Occupation { get; set; }

        [BindProperty]
        [Required]
        public string? Title { get; set; }

        [BindProperty]
        [Required]
        public string? Description { get; set; }

        [BindProperty]
        public IFormFile? Image { get; set; }
    }

    public class TestimonialsModel : PageModel
    {
        private readonly RazorPagesContext context;
        public TestimonialForm InputForm { get; set; } = default!;
        public List<Testimonial> Testimonials { get; set; } = new();
        private readonly string rootPath = "wwwroot";
        private readonly string uploadsPath = "uploads";

        public TestimonialsModel(RazorPagesContext context)
        {
            this.context = context;
            Testimonials = this.context.Testimonials.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var pathToImage = Path.Combine(uploadsPath, "default_image.png");
            if (InputForm.Image != null)
            {
                var fileName = Guid.NewGuid().ToString() + ".png";
                var relativePath = Path.Combine(uploadsPath, fileName);
                var fullPath = Path.Combine(rootPath, relativePath);
                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await InputForm.Image.CopyToAsync(fileStream);
                }
                Utils.ClipToCircle(fullPath);
                pathToImage = relativePath;
            }

            context.Testimonials.Add(new Testimonial
            {
                Id = 0,
                Name = InputForm.Name,
                Occupation = InputForm.Occupation,
                Title = InputForm.Title,
                Description = InputForm.Description,
                ImgUrl = pathToImage
            });
            await context.SaveChangesAsync();
            return RedirectToPage("/Testimonials");
        }
    }
}
