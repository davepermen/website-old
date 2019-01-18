using EvState.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class StopModel : PageModel
    {
        public async Task<IActionResult> OnGet([FromServices] ECarUpHttpClient eCarUpHttpClient)
        {
            await eCarUpHttpClient.StopCharging();
            return Redirect("/");
        }
    }
}