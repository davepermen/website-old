using EvState.HttpClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class IndexModel : PageModel
    {
        public ChargingState[] ChargingState { get; private set; }
        public float StateOfCharge { get; private set; }

        public async Task OnGet([FromServices] ECarUpHttpClient eCarUpHttpClient, [FromServices] EVNotifyHttpClient evNotifyHttpClient)
        {
            var chargingState = await eCarUpHttpClient.State();

            var stateOfCharge = await evNotifyHttpClient.GetStateOfCharge();

            this.ChargingState = chargingState;

            this.StateOfCharge = stateOfCharge;
        }
    }
}
