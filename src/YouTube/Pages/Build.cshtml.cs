using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IO = System.IO;

namespace YouTube.Pages
{
    public class BuildModel : PageModel
    {
        public async Task OnGet()
        {
            using (var http = new HttpClient())
            {
                var feeds = SubscriptionManager.LoadFromFile(@"wwwroot\subscription_manager.xml");

                var allVideos = new List<Xml.feedEntry>();

                foreach (var feed in feeds)
                {
                    try
                    {
                        using (var stream = await http.GetStreamAsync(feed.xmlUrl))
                        {
                            allVideos.AddRange(Subscription.ReadFeed(stream));
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                var pages = allVideos.OrderByDescending(v => v.published).Select((e, i) => new { Entry = e, Index = i }).GroupBy(e => e.Index / 10, x => x.Entry);

                foreach (var page in pages)
                {
                    var document = page.Key == 0 ? @"wwwroot\index.html" : $@"wwwroot\{page.Key}\index.html";
                    if (page.Key != 0)
                    {
                        IO.Directory.CreateDirectory($@"wwwroot\{page.Key}");
                    }

                    IO.File.WriteAllText(document, "<!doctype html>\n" +
                        "<html>\n" +
                        "   <head>\n" +
                        "       <meta charset='utf-8'>\n" +
                        "       <title>youtube</title>\n" +
                        "       <link href='/style.css' rel='stylesheet'>\n" +
                        "       <script>function youtube_check(e) { var thumbnail = ['maxresdefault', 'mqdefault', 'sddefault', 'hqdefault', 'default']; var url = e.getAttribute('src'); if (e.naturalWidth === 120 && e.naturalHeight === 90) { for (var i = 0, len = thumbnail.length - 1; i < len; i++) { if (url.indexOf(thumbnail[i]) > 0) { e.setAttribute('src', url.replace(thumbnail[i], thumbnail[i + 1])); break; } } } }</script>\n" +
                        "   </head>\n" +
                        "   <body>\n");

                    foreach (var video in page.OrderByDescending(v => v.published).Take(20))
                    {
                        IO.File.AppendAllText(document, $"      <a href='https://www.youtube-nocookie.com/embed/{video.videoId}?autoplay=1'>\n" +
                                                        $"          <img src='https://i3.ytimg.com/vi/{video.videoId}/maxresdefault.jpg' onload='youtube_check(this)'>\n" +
                                                        $"      </a>\n");
                    }

                    if (page.Key != pages.Last().Key)
                    {
                        IO.File.AppendAllText(document, $"      <a href='/{page.Key + 1}/'></a>\n");
                    }

                    IO.File.AppendAllText(document, "   </body>\n</html>");
                }
            }
        }
    }
}