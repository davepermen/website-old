using EvState.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class StartModel : PageModel
    {
        public async Task<IActionResult> OnGet([FromServices] ECarUpHttpClient eCarUpHttpClient)
        {
            await eCarUpHttpClient.StartCharging(TimeSpan.FromHours(1));
            return Redirect("/");
        }
    }
}