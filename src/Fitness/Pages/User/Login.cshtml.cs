using Conesoft.DataSources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using IO = System.IO;

namespace Fitness.Pages.User
{
    public class LoginModel : PageModel
    {
        public async Task<IActionResult> OnPost([FromServices] IDataSources dataSources)
        {
            if (ModelState.IsValid)
            {
                Username = Username.ToLower();

                var userDirectory = $"{dataSources.SharedDirectory}/users-legacy";
                var userFile = $"{userDirectory}/{Username}.txt";

                if (IO.File.Exists(userFile) == true)
                {
                    var passwordHasher = new PasswordHasher<string>();

                    var hashed = IO.File.ReadAllText(userFile);

                    var validLogin = passwordHasher.VerifyHashedPassword(Username, hashed, Password);

                    if (validLogin == PasswordVerificationResult.Success)
                    {
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
            if(Request.Form.ContainsKey("redirectto"))
            {
                return Redirect(Request.Form["redirectto"]);
            }
            return Redirect("/");
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