using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YouTube.Services;
using IO = System.IO;

namespace YouTube.Pages
{
    public class BuildModel : PageModel
    {
        public async Task OnGet([FromServices] IDataSources dataSource, [FromServices] ThumbnailCache thumbnailCache)
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

                IO.File.WriteAllLines($@"{dataSource.LocalDirectory}\videos.csv", allVideos.OrderByDescending(v => v.published).Select(v => $"{v.videoId};{v.author.name};{v.title}").ToArray());

                using (var client = new HttpClient())
                {
                    foreach (var video in allVideos)
                    {
                        if (await thumbnailCache.CacheThumbnail(video.videoId))
                        {
                            var message = new
                            {
                                title = $"New Video by {video.author.name}",
                                message = $"{video.title} by {video.author.name} is now available",
                                image_url = $"https://yt.davepermen.net/thumb/{video.videoId}.jpg",
                                action = $"https://yt.davepermen.net/{video.videoId}",
                                type = "YouTube"
                            };

                            await client.GetAsync($@"https://wirepusher.com/send?id=mpgpt&title={message.title}&message={message.message}&type={message.type}&image_url={message.image_url}&action={message.action}");
                        }
                    }
                }
            }
        }
    }
}