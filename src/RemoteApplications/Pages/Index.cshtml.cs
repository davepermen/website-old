using Conesoft.DataSources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RemoteApplications.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet([FromServices] IConfiguration configuration, [FromServices] IDataSources dataSources)
        {
            var applications = Directory.GetFiles(dataSources.LocalDirectory, "*.rdp").Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

            AllApplictions = applications.Select(file =>
            {
                var publishDate = new FileInfo(Path.Combine(dataSources.LocalDirectory, $"{file}.rdp")).CreationTimeUtc;

                var icons = new List<Icon>();
                if (System.IO.File.Exists(Path.Combine(dataSources.LocalDirectory, $"{file}.ico")))
                {
                    icons.Add(new Icon { Type = "Ico", TagType = "IconRaw", File = $"{file}.ico", Size = null });
                }
                if (System.IO.File.Exists(Path.Combine(dataSources.LocalDirectory, $"{file}.png")))
                {
                    icons.Add(new Icon { Type = "Png", TagType = "Icon32", File = $"{file}.png", Size = "32x32" });
                }

                return new Application
                {
                    Name = file,
                    Icons = icons.ToArray(),
                    PublishDate = publishDate
                };
            }).ToArray();

            PublishDate = AllApplictions.Max(application => application.PublishDate);

            RemoteServer = configuration["server"];
        }

        public Application[] AllApplictions { get; private set; }

        public DateTime PublishDate { get; private set; }

        public string RemoteServer { get; private set; }
    }
}