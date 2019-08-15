using Conesoft;
using Microsoft.AspNetCore.Mvc;
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
        public async Task OnGet([FromServices] IDataSources dataSource)
        {
            using (var http = new HttpClient())
            {
                var feeds = SubscriptionManager.LoadFromFile($@"{dataSource.LocalDirectory}\subscription_manager.xml");

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

                IO.File.WriteAllLines($@"{dataSource.LocalDirectory}\videos.csv", allVideos.OrderByDescending(v => v.published).Select(v => v.videoId).ToArray());
            }
        }
    }
}