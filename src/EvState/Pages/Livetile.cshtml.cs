using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvState.Pages
{
    public class LivetileModel : PageModel
    {
        private int amount;

        public void OnGet([FromServices] IDataSources dataSources)
        {
            Response.ContentType = "text/xml";
        }
    }
}