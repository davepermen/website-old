using Conesoft;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IO = System.IO;

namespace YouTube.Services
{
    public class ThumbnailCache
    {
        readonly IDataSources dataSource;
        readonly HttpClient http;

        public ThumbnailCache([FromServices] IDataSources dataSource)
        {
            this.dataSource = dataSource;
            this.http = new HttpClient();
        }

        public async Task<bool> CacheThumbnail(string videoId)
        {
            IO.Directory.CreateDirectory($@"{dataSource.LocalDirectory}\thumbnails");

            var thumb = $@"{dataSource.LocalDirectory}\thumbnails\{videoId}.jpg";

            if (IO.File.Exists(thumb) == false)
            {
                var defaults = new[] { "maxresdefault", "mqdefault", "sddefault", "hqdefault", "default" };
                foreach (var current in defaults)
                {
                    var url = $@"https://i3.ytimg.com/vi/{videoId}/{current}.jpg";
                    try
                    {
                        IO.File.WriteAllBytes(thumb, await http.GetByteArrayAsync(url));
                        break;
                    }
                    catch (Exception)
                    {
                        IO.File.Copy($@"{dataSource.LocalDirectory}\default.jpg", thumb);
                    }
                }
                return true;
            }

            return false;
        }
    }
}
