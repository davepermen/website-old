using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using IO = System.IO;

namespace YouTube.Pages
{
    public class WidgetModel : PageModel
    {
        public void OnGet([FromServices] IDataSources dataSource)
        {
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

        public Video[] Videos { get; set; } = new Video[] { };

        public class Video
        {
            public string Id { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
        }
    }
}