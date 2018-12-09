using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Homepage.Pages
{
    public class BlogModel : PageModel
    {
        static readonly MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseYamlFrontMatter().Build();
        string blogContent = null;

        public bool HasBlogContent => blogContent != null;
        public string BlogContent => blogContent ?? "";


        public async Task<IActionResult> OnGet()
        {
            if (RouteData.Values["Slug"] is string slug)
            {
                var path = $@"{Program.DataRoot}\blog\{slug}\index.md";
                if (path.Contains(@"\..\") == false && System.IO.File.Exists(path))
                {
                    var markdown = await System.IO.File.ReadAllTextAsync(path);
                    var blog = Markdown.ToHtml(markdown, pipeline);

                    var hero = $"/blog/{slug}/hero.jpg";

                    blogContent = AddSections(blog.ReplaceFirst("<h1>", $"<h1 style=\"background-image: url('{hero}')\">"));
                    return Page();
                }
                return NotFound($"route '{slug}' is not a valid blogpost");
            }
            throw new Exception("RouteData dependency on 'Slug' in Blog.cshtml  is not working anymore");
        }

        // yes. yes it works. no, i won't comment.
        static string AddSections(string html) => Regex.Replace(html.Replace("<p>", "<section><p>").Replace("</p>", "</p></section>"), @"</section>\s+<section>", "");
    }
}