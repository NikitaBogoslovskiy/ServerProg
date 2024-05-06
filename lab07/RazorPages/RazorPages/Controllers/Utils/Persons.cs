using RazorPages.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Controllers.Utils
{
    public static class Persons
    {
        public static string DefaultImagePath = "~/media/persons_images/default_image.png";
        private static string uploadsPath = "media/persons_images";
        private static string rootFolder = "wwwroot";

        public static string? GetImagePath(IFormFile? image)
        {
            if (image == null)
                return null;
            var fileName = Guid.NewGuid().ToString() + ".png";
            var relativePath = Path.Combine(uploadsPath, fileName);
            var fullPath = Path.GetFullPath(Path.Combine(rootFolder, relativePath));
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }
            return "~/" + relativePath;
        }

        public static void DeleteImage(string imagePath) => File.Delete(Path.GetFullPath(Path.Combine(rootFolder, imagePath[2..])));

        public class PersonForm
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? ImagePath { get; set; }
            public IFormFile? Image { get; set; }
        }
    }
}
