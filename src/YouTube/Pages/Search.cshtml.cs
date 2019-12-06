using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YouTube.Services;

namespace YouTube.Pages
{
    public class SearchModel : PageModel
    {
        public async Task OnGet(string query, [FromServices] ThumbnailCache thumbnailCache)
        {
            HasQuery = string.IsNullOrWhiteSpace(query) == false;
            Query = HasQuery ? query : null;

            if (HasQuery)
            {
                var client = new HttpClient();
                var result = await client.GetAsync(@"https://www.youtube.com/results?search_query=" + query);

                var content = await result.Content.ReadAsStringAsync();

                var hrefsPlusGarbage = content.Split("href=\"/watch?");

                var hrefs = hrefsPlusGarbage
                    .Where(line => line.StartsWith("v=")).ToArray();

                var elements = hrefs.Select(line =>
                {
                    var id = line.Split('\"').FirstOrDefault().Substring("v=".Length);

                    var potentialTitle = line.Split("title=\"").Skip(1).ToArray();
                    var title = potentialTitle.Any() ? potentialTitle.First().Split('\"').FirstOrDefault() : "";

                    var potentialAuthor = line.Split("<a href=\"/user/").Skip(1).ToArray();
                    var potentialAuthorName = potentialAuthor.Any() ? potentialAuthor.First().Split('>').Skip(1).ToArray() : Array.Empty<string>();
                    var authorNameWithGarbage = potentialAuthorName.Any() ? potentialAuthorName.First().Split("</a>").FirstOrDefault() : "";
                    var author = authorNameWithGarbage.Any() ? authorNameWithGarbage.Substring(0, authorNameWithGarbage.IndexOf("</")) : "";

                    return new Video
                    {
                        Id = id,
                        Title = title,
                        Author = author
                    };
                });

                Videos = elements
                    .Where(video => string.IsNullOrWhiteSpace(video.Title) == false)
                    .Where(video => string.IsNullOrWhiteSpace(video.Author) == false)
                    .Where(video => video.Id.Contains("list=") == false)
                    .ToArray();

                await Task.WhenAll(Videos.Select(video => thumbnailCache.CacheThumbnail(video.Id)));
            }
        }

        public bool HasQuery { get; set; } = false;
        public string Query { get; set; }
        public Video[] Videos { get; set; }

        public class Video
        {
            public string Id { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
        }
    }
}