using Microsoft.AspNetCore.Mvc;
using RazorPages.Controllers.Api;
using System.ComponentModel.DataAnnotations;

namespace RazorPages.Controllers.Utils
{
    public static class UsersData
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
                var loginIsValid = UsersData.Users.ContainsKey(inputUser.Login ?? "");
                if (!loginIsValid)
                {
                    ErrorMessage = "Login does not exist";
                    return false;
                }

                var obtainedUser = UsersData.Users[inputUser.Login];
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
}
