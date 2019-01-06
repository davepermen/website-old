using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using IO = System.IO;

namespace Fitness.Pages
{
    public class MenuModel : PageModel
    {
        private int[] years;
        private string[] users;
        private string[] trainings;

        public void OnGet([FromServices] IDataSources dataSources)
        {
            years = IO.Directory.GetDirectories(dataSources.LocalDirectory).Select(year => int.Parse(IO.Path.GetFileName(year))).ToArray();
            users = years.SelectMany(year => IO.Directory.GetDirectories($"{dataSources.LocalDirectory}/{year}").Select(user => IO.Path.GetFileName(user))).Distinct().ToArray();
            trainings = years.SelectMany(year =>
            {
                var users = IO.Directory.GetDirectories($"{dataSources.LocalDirectory}/{year}").Select(user => IO.Path.GetFileName(user));
                return users.SelectMany(user => IO.Directory.GetDirectories($"{dataSources.LocalDirectory}/{year}/{user}")).Select(training => IO.Path.GetFileName(training));
            }).Distinct().ToArray();
        }

        public int[] Years => years;
        public string[] Users => users;
        public string[] Trainings => trainings;
    }
}