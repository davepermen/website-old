using Conesoft.DataSources;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homepage.Pages
{
    public class BlogModel : PageModel
    {
        static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().Build();

        public string BlogContent { get; private set; } = null;
        public Metadata_ Metadata { get; private set; } = null;
        public string HeroImage { get; private set; } = null;
        public bool HasContent => BlogContent != null;


        public async Task<IActionResult> OnGet(string slug, [FromServices] IDataSources dataSources)
        {
            var path = $@"{dataSources.LocalDirectory}\blog\{slug}\index.md";
            if (slug.Contains(@"\") == false && slug.Contains(@"..") == false && System.IO.File.Exists(path))
            {
                this.HeroImage = $"/blog/{slug}/hero.jpg";

                var markdown = await System.IO.File.ReadAllTextAsync(path);
                var blog = Markdown.ToHtml(markdown, pipeline);
                this.BlogContent = AddSections(blog);

                this.Metadata = new Metadata_(markdown);

                return Page();
            }
            return NotFound($"route '{slug}' is not a valid blogpost");
        }

        // yes. yes it works. no, i won't comment.
        static string AddSections(string html) => Regex.Replace(html.Replace("<p>", "<section><p>").Replace("</p>", "</p></section>"), @"</section>\s+<section>", "");

        public class Metadata_
        {
            public string Author { get; set; }
            public DateTime Date { get; set; }
            public string Title { get; set; }

            public Metadata_(string markdown)
            {
                var splits = markdown.Split(new string[] { "---" }, StringSplitOptions.None);
                if (splits.Length == 3)
                {
                    var metadata = splits[1];

                    foreach (var line in metadata.Split('\n').Select(l => l.Trim()))
                    {
                        var keyValue = line.Split(':').Select(l => l.Trim()).ToArray();
                        if (keyValue.Length == 2)
                        {
                            var key = keyValue[0];
                            var value = keyValue[1];
                            TrySet(key, value);
                        }
                    }
                }
            }

            private void TrySet(string key, string value)
            {
                switch (key.ToLower())
                {
                    case "author":
                        Author = value;
                        break;

                    case "date":
                        if (DateTime.TryParse(value, out var date))
                        {
                            Date = date;
                        }
                        break;

                    case "title":
                        Title = value;
                        break;
                }
            }
        }
    }
}