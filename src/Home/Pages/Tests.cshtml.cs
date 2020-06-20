using Home.Data.Tesla;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Home.Pages
{
    public class TestsModel : PageModel
    {
        public Vehicle? Vehicle { get; set; }
        public State? State { get; set; }
        public async Task OnGet([FromServices] Services.HttpClients.Tesla.Client client)
        {
            Vehicle = await client.GetVehicle("Epona");
            State = Vehicle != null ? await client.GetStateOf(Vehicle) : null;
        }
    }
}