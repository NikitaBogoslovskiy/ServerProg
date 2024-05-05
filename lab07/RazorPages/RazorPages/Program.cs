using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NuGet.Common;
using RazorPages.Controllers;
using RazorPages.Controllers.Utils;
using RazorPages.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = Auth.TokenValidationParameters;
    });
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<MoviesContext>();
builder.Services.AddSession();
builder.Services.AddMvc().AddJsonOptions(options => {
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.Use(async (context, next) =>
{
    var token = Auth.GetTokenFromCookies(context);
    if (!string.IsNullOrEmpty(token))
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            context.User = tokenHandler.ValidateToken(token, Auth.TokenValidationParameters, out var securityToken);
        }
        catch(SecurityTokenExpiredException) 
        {
            token = Auth.ConvertJwtToToken(Auth.GenerateJwt(tokenHandler.ReadJwtToken(token).Claims.ToList()));
            Auth.AddTokenToCookies(context, token);
            context.User = tokenHandler.ValidateToken(token, Auth.TokenValidationParameters, out var securityToken);
        }
        catch (Exception)
        {
            Auth.DeleteTokenFromCookies(context);
        }
    }
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
