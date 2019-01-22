using EvState.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class TileModel : PageModel
    {
        public async Task OnGet([FromServices] EVNotifyHttpClient evNotifyHttpClient)
        {
            StateOfCharge = await evNotifyHttpClient.GetStateOfCharge();
        }

        public float StateOfCharge { get; set; }

        public float Charged { get; set; }
    }
}