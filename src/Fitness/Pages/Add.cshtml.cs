using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IO = System.IO;

namespace Fitness.Pages
{
    public class AddModel : PageModel
    {
        readonly IDataSources dataSources;

        public AddModel(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        readonly List<int> dailyPushups = new List<int>();
        readonly List<int> summedPushups = new List<int>();

        public string Me { get; set; }

        public int Pushups { get; set; } = 0;

        public void OnGet()
        {
            string training = "pushups";
            string user = "davepermen";

            var path = $@"{dataSources.LocalDirectory}\{training}\{user}";
            if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*.txt"))
                {
                    var pushup = int.Parse(IO.File.ReadAllText(file));
                    dailyPushups.Add(pushup);
                    Pushups += pushup;
                    summedPushups.Add(Pushups);
                }
            }
        }

        public IActionResult OnPost(int pushups)
        {
            if (User.Identity.IsAuthenticated)
            {
                string training = "pushups";
                string user = User.Identity.Name;

                Log(pushups, training, user);

                var path = $@"{dataSources.LocalDirectory}\{training}\{user}";
                if (Directory.Exists(path))
                {
                    foreach (var file in Directory.GetFiles(path, "*.txt"))
                    {
                        var pushup = int.Parse(IO.File.ReadAllText(file));
                        dailyPushups.Add(pushup);
                        Pushups += pushup;
                        summedPushups.Add(Pushups);
                    }
                }
                return Page();
            }
            else
            {
                return Unauthorized();
            }
        }

        private void Log(int pushups, string training, string user)
        {
            IO.Directory.CreateDirectory($@"{dataSources.LocalDirectory}\{training}");
            IO.Directory.CreateDirectory($@"{dataSources.LocalDirectory}\{training}\{user}");

            var filename = $@"{dataSources.LocalDirectory}\{training}\{user}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = IO.File.Exists(filename) ? int.Parse(IO.File.ReadAllText(filename)) : 0;
            IO.File.WriteAllText(filename, $"{current + pushups}");

            UpdateLiveTile(new HomeControllerModel(training, user, dataSources));
        }

        private void UpdateLiveTile(HomeControllerModel model)
        {
            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{amount}}", model.Pushups.ToString());
            IO.File.WriteAllText("wwwroot\\livetile.xml", content);
        }
    }
}