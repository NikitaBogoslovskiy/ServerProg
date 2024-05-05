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
using System.Security.Claims;
using System.Text;

namespace RazorPages.Controllers.Api
{
    [Route("api")]
    [ApiController]
    public class LoginControllerApi : ControllerBase
    {
        private readonly MoviesContext _context;

        public LoginControllerApi(MoviesContext context)
        {
            _context = context;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> GetToken([LoginFormValidation] UserForm userForm)
        {
            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var claims = new List<Claim>();
            claims.AddRange(userForm.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
            var jwt = Auth.GenerateJwt(claims, TimeSpan.FromDays(7));
            return Ok(Auth.ConvertJwtToToken(jwt));
        }

        [HttpPost("register")]
        public async Task<IActionResult> SignUp([RegisterFormValidation] UserForm userForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            foreach (var role in userForm.Roles)
            {
                _context.Add(new User()
                {
                    Login = userForm.Login,
                    Password = userForm.Password,
                    Role = role
                });
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
