using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using RazorPages.Controllers.Data;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(User user)
        {
            if (!ModelState.IsValid)
                return View("Index", user);

            var claims = new List<Claim>();
            claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
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
    }
}
