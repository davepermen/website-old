﻿using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IO = System.IO;

namespace YouTube.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet(int? index, [FromServices] IDataSources dataSource)
        {
            Index = index ?? 0;

            try
            {
                VideoIds = IO.File.ReadAllLines($@"{dataSource.LocalDirectory}\videos.csv");
            }
            catch
            {
                VideoIds = new string[] { };
            }
        }

        public int Index { get; set; }

        public string[] VideoIds { get; set; }
    }
}