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
        private readonly IConfiguration configuration;

        public IndexModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnGet()
        {
            var applications = Directory.GetFiles(Program.DataRoot, "*.rdp").Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();

            AllApplictions = applications.Select(file =>
            {
                var publishDate = new FileInfo(Path.Combine(Program.DataRoot, $"{file}.rdp")).CreationTimeUtc;

                var icons = new List<Icon>();
                if (System.IO.File.Exists(Path.Combine(Program.DataRoot, $"{file}.ico")))
                {
                    icons.Add(new Icon { Type = "Ico", TagType = "IconRaw", File = $"{file}.ico", Size = null });
                }
                if (System.IO.File.Exists(Path.Combine(Program.DataRoot, $"{file}.png")))
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

    public class Application
    {
        public string Name { get; set; }
        public string RdpName => Name + ".rdp";
        public Icon[] Icons { get; set; }
        public DateTime PublishDate { get; set; }
    }

    public class Icon
    {
        public string Type { get; set; }
        public string TagType { get; set; }
        public string File { get; set; }
        public string Size { get; set; }
    }
}