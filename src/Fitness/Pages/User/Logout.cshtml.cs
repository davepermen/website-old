using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Fitness.Pages.User
{
    public class LogoutModel : PageModel
    {
        public async Task OnGet()
        {
            await HttpContext.SignOutAsync();
        }

        public async Task OnPost()
        {
            await HttpContext.SignOutAsync();
        }
    }
}