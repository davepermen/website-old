using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
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
                    Videos = IO.File.ReadAllLines($@"{dataSource.LocalDirectory}\videos.csv").Select(line =>
                    {
                        var splits = line.Split(";");
                        return new Video
                        {
                            Id = splits[0],
                            Author = splits[1],
                            Title = splits[2]
                        };
                    }).ToArray();
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

        public Video[] Videos { get; set; } = new Video[] { };


        public class Video
        {
            public string Id { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
        }
    }
}