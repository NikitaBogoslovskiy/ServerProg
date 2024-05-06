using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RazorPages.Controllers.Utils;
using RazorPages.Data;
using RazorPages.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RazorPages.Controllers
{
    public class LoginController : Controller
    {
        private readonly MoviesContext _context;

        public LoginController(MoviesContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn([LoginFormValidation] UserForm userForm)
        {
            if (!ModelState.IsValid)
                return View("Index", userForm);

            var claims = new List<Claim>();
            claims.AddRange(userForm.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
            var jwt = Auth.GenerateJwt(claims);
            Auth.AddTokenToCookies(HttpContext, Auth.ConvertJwtToToken(jwt));
            return RedirectToAction("Index", "Movies");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            HttpContext.Response.Cookies.Delete("Token");
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([RegisterFormValidation] UserForm userForm)
        {
            if (!ModelState.IsValid)
                return View("Index", userForm);

            foreach (var role in userForm.Roles)
            {
                _context.Add(new User()
                {
                    Id = 0,
                    Login = userForm.Login,
                    Password = userForm.Password,
                    Role = role
                });
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}
