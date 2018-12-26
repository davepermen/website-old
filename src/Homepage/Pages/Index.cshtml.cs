using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Homepage.Pages
{
    public class IndexModel : PageModel
    {
        readonly DataSources.Root root;

        public IndexModel(DataSources.Root root)
        {
            this.root = root;
        }
        public IEnumerable<(string Title, string Path)> BlogPosts { get; private set; } = null;

        public void OnGet()
        {
            BlogPosts = Directory.GetDirectories($@"{root.LocalDirectory}\blog").Select(p =>
            {
                var name = Path.GetFileName(p);

                return (Title: name.Replace('-', ' '), Path: name);
            });
        }
    }
}