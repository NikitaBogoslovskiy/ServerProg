using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RazorPages.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RazorPages.Controllers.Api
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        const string KEY = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(KEY));
    }

    public static class Data
    {
        public static Dictionary<string, User> Users = new()
        {
            ["alice"] = new User("alice", "123", new List<string> { "User" }),
            ["bob"] = new User("bob", "456", new List<string> { "User", "Admin" })
        };
    }

    public class UserValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is User inputUser)
            {
                var loginIsValid = Data.Users.ContainsKey(inputUser.Login ?? "");
                if (!loginIsValid)
                {
                    ErrorMessage = "Login does not exist";
                    return false;
                }

                var obtainedUser = Data.Users[inputUser.Login];
                var passwordIsValid = obtainedUser.Password == inputUser.Password;
                if (!passwordIsValid)
                {
                    ErrorMessage = "Password is incorrect";
                    return false;
                }

                inputUser.Roles = obtainedUser.Roles;
                return true;
            }
            else
                return false;
        }
    }


    [UserValidation]
    public class User
    {
        [BindProperty]
        [Required]
        public string? Login { get; set; }

        [BindProperty]
        [Required]
        public string? Password { get; set; }

        public List<string>? Roles { get; set; }

        public User() { }
        public User(string login, string password, List<string> roles) { Login = login; Password = password; Roles = roles; }
    }

    public class LoginControllerApi : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(User user)
        {
            if (!ModelState.IsValid)
                return View("Index", user);

            var claims = new List<Claim>();
            claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            HttpContext.Session.SetString("Token", token);
            return RedirectToAction("Index", "Movies");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index", "Login");
        }
    }
}
