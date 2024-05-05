using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RazorPages.Controllers.Api;
using RazorPages.Data;
using RazorPages.Models;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Controllers.Utils
{
    public class UserForm
    {
        [BindProperty]
        [Required]
        public string? Login { get; set; }

        [BindProperty]
        [Required]
        public string? Password { get; set; }

        public List<string>? Roles { get; set; }

        public UserForm() { }
        public UserForm(string login, string password, List<string> roles) { Login = login; Password = password; Roles = roles; }
    }

    public class LoginFormValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(MoviesContext)) as MoviesContext;
            if (value is UserForm userForm)
            {
                var foundUsers = _context.Users.Where(u => u.Login == userForm.Login).ToList();

                if (!foundUsers.Any())
                    return new ValidationResult("Login does not exist");

                var hash = Auth.HashPassword(userForm.Password);
                if (hash != foundUsers.First().Password)
                    return new ValidationResult("Password is incorrect");

                userForm.Password = hash;
                userForm.Roles = foundUsers.Select(u => u.Role).ToList();
                return ValidationResult.Success;
            }
            else
                return new ValidationResult("User attributes are incorrect");
        }
    }

    public class RegisterFormValidation : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(MoviesContext)) as MoviesContext;
            if (value is UserForm userForm)
            {
                if (_context.Users.Where(u => u.Login == userForm.Login).Any())
                {
                    return new ValidationResult("Login already exists");
                }
                userForm.Password = Auth.HashPassword(userForm.Password);
                userForm.Roles = new List<string>() { "User" };
                return ValidationResult.Success;
            }
            else
                return new ValidationResult("User attributes are incorrect");
        }
    }
}
