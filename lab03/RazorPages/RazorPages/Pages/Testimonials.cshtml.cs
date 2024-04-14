using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPages.Data;
using RazorPages.Models;

namespace RazorPages.Pages
{
    public class TestimonialsModel : PageModel
    {
        private readonly RazorPagesContext context;
        public List<Testimonial> Testimonials { get; set; } = new();

        public TestimonialsModel(RazorPagesContext context)
        {
            this.context = context;
            Testimonials = this.context.Testimonials.ToList();
        }
    }
}
