using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RazorPages.Controllers.Data
{
    public static class Auth
    {
        private static string issuer = "AuthServer";
        private static string audience = "AuthClient";
        private static string key = "mysupersecret_secretsecretsecretkey!123";   // ключ для шифрации
        private static SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(key));
        private static TimeSpan tokenLifetime = TimeSpan.FromMinutes(5);
        private static TimeSpan cookieLifetime = TimeSpan.FromDays(3);

        public static HashSet<string> LoginPaths = new() { "/Login/Index", "/Login/SignIn", "/Login/SignOut" };

        public static TokenValidationParameters TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            IssuerSigningKey = GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };

        public static JwtSecurityToken GenerateJwt(List<Claim>? claims, TimeSpan? lifetime = null)
        {
            return new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(lifetime ?? tokenLifetime),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );
        }

        public static string ConvertJwtToToken(JwtSecurityToken jwt) => new JwtSecurityTokenHandler().WriteToken(jwt);

        public static void AddTokenToCookies(HttpContext context, string token)
        {
            context.Response.Cookies.Append("Token", token, new CookieOptions
            {
                Expires = DateTime.UtcNow.Add(cookieLifetime)
            });
        }

        public static string? GetTokenFromCookies(HttpContext context) => context.Request.Cookies["Token"];

        public static void DeleteTokenFromCookies(HttpContext context) => context.Response.Cookies.Delete("Token");
    }
}
