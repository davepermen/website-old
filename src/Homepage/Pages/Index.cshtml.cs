﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Homepage.Pages
{
    public class IndexModel : PageModel
    {
        public IEnumerable<(string Title, string Path)> BlogPosts { get; private set; } = null;

        public void OnGet()
        {
            BlogPosts = Directory.GetDirectories($@"{Program.DataRoot}\blog").Select(p =>
            {
                var name = Path.GetFileName(p);

                return (Title: name.Replace('-', ' '), Path: name);
            });
        }
    }
}