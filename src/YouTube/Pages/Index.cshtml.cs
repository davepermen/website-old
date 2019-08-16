using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IO = System.IO;

namespace YouTube.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet(string indexOrId, [FromServices] IDataSources dataSource)
        {
            IsIndex = int.TryParse(indexOrId, out int index) || indexOrId == null;
            if (IsIndex)
            {
                Index = index;
                try
                {
                    VideoIds = IO.File.ReadAllLines($@"{dataSource.LocalDirectory}\videos.csv");
                }
                catch
                {
                }
            }
            else
            {
                VideoId = indexOrId;
            }
        }

        public bool IsIndex { get; set; } = false;

        public int Index { get; set; } = 0;

        public string VideoId { get; set; } = null;

        public string[] VideoIds { get; set; } = new string[] { };
}
}