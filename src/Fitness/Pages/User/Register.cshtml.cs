using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using IO = System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Conesoft;

namespace Fitness.Pages.User
{
    public class RegisterModel : PageModel
    {
        public async Task OnPost([FromServices] IDataSources dataSources)
        {
            if (ModelState.IsValid)
            {
                Username = Username.ToLower();

                var userDirectory = $"{dataSources.SharedDirectory}/users-legacy";
                var userFile = $"{userDirectory}/{Username}.txt";

                IO.Directory.CreateDirectory(userDirectory);
                if (IO.File.Exists(userFile) == false)
                {
                    var passwordHasher = new PasswordHasher<string>();
                    var hashed = passwordHasher.HashPassword(Username, Password);

                    IO.File.WriteAllText(userFile, hashed);

                    // Create the identity from the user info
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Username));
                    identity.AddClaim(new Claim(ClaimTypes.Name, Username));

                    // Authenticate using the identity
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                }
            }
        }

        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}