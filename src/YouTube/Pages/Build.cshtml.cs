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

                IO.File.WriteAllLines($@"{dataSource.LocalDirectory}\videos.csv", allVideos.OrderByDescending(v => v.published).Select(v => $"{v.videoId};{v.author.name};{v.title}").ToArray());

                using (var client = new HttpClient())
                {
                    foreach (var video in allVideos)
                    {
                        IO.Directory.CreateDirectory($@"{dataSource.LocalDirectory}\thumbnails");

                        var thumb = $@"{dataSource.LocalDirectory}\thumbnails\{video.videoId}.jpg";

                        if (IO.File.Exists(thumb) == false)
                        {
                            var defaults = new[] { "maxresdefault", "mqdefault", "sddefault", "hqdefault", "default" };
                            foreach (var current in defaults)
                            {
                                var url = $@"https://i3.ytimg.com/vi/{video.videoId}/{current}.jpg";
                                try
                                {
                                    IO.File.WriteAllBytes(thumb, await http.GetByteArrayAsync(url));
                                    break;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            var message = new
                            {
                                title = $"New Video by {video.author.name}",
                                message = $"{video.title} by {video.author.name} is now online",
                                image_url = $"https://yt.davepermen.net/thumb/{video.videoId}.jpg",
                                action = $"https://yt.davepermen.net/{video.videoId}",
                                type = "YouTube"
                            };

                            await client.GetAsync($@"https://wirepusher.com/send?id=mpgpt&title={message.title}&message={message.message}&type={message.type}&image_url={message.image_url}&action={message.action}");
                        }
                    }
                }

                foreach (var file in IO.Directory.GetFiles($@"{dataSource.LocalDirectory}\thumbnails", "*.jpg"))
                {
                    if (allVideos.Any(v => v.videoId == IO.Path.GetFileNameWithoutExtension(file)) == false)
                    {
                        IO.File.Delete(file);
                    }
                }
            }
        }
    }
}