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
using System.Security.Claims;
using System.Text;

namespace RazorPages.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class LoginControllerApi : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> GetToken(User user)
        {
            if (!ModelState.IsValid)
                return NotFound(ModelState);

            var claims = new List<Claim>();
            claims.AddRange(user.Roles.Select(x => new Claim(ClaimTypes.Role, x)));
            var jwt = Auth.GenerateJwt(claims, TimeSpan.FromDays(7));
            return Ok(Auth.ConvertJwtToToken(jwt));
        }
    }
}
