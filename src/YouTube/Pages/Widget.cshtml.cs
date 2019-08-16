using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IO = System.IO;

namespace YouTube.Pages
{
    public class WidgetModel : PageModel
    {
        public void OnGet([FromServices] IDataSources dataSource)
        {
            try
            {
                VideoIds = IO.File.ReadAllLines($@"{dataSource.LocalDirectory}\videos.csv");
            }
            catch
            {
            }
        }

        public string[] VideoIds { get; set; } = new string[] { };
    }
}