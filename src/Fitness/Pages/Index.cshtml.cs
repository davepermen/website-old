using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IO = System.IO;

namespace Fitness.Pages
{
    public class IndexModel : PageModel
    {
        readonly List<int> dailyPushups = new List<int>();
        readonly List<int> summedPushups = new List<int>();

        public string Me { get; set; }

        public int Pushups { get; set; } = 0;

        public void OnGet([FromServices] IDataSources dataSources)
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
        public string DailyPushupsGraph => string.Join(" ", dailyPushups.Select((value, index) => $"{index}, {value * .5f}")); // format for svg polyline
        public string DailyPushupsGraphBackground => DailyPushupsGraph + " 364, 0";
        public string SummedPushupsGraph => string.Join(" ", summedPushups.Select((value, index) => $"{index}, {value * .02f}")); // format for svg polyline
        public string SummedPushupsGraphBackground => SummedPushupsGraph + " 364, 0";
    }
}