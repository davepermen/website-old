using Conesoft;
using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;

namespace Fitness.Pages
{
    public class IndexModel : PageModel
    {
        TrainingData trainingData;

        public int Pushups => trainingData.Sum;

        public int YearGoal => trainingData.Goal;

        public void OnGet([FromQuery] int? year, [FromServices] IDataSources dataSources)
        {
            var training = "pushups";
            var user = User.Identity.IsAuthenticated ? User.Identity.Name : "davepermen";

            year = year ?? DateTime.Now.Year;

            trainingData = new TrainingData(dataSources, user, training, year.Value);
        }

        public string DailyPushupsGraph => string.Join(" ", trainingData.AmountPerDay.Select((value, index) => $"{index}, {value * .5f}")); // format for svg polyline
        public string DailyPushupsGraphBackground => "0, 0 " + DailyPushupsGraph + " 364, 0";
        public string SummedPushupsGraph => string.Join(" ", trainingData.AccumulatedEveryDay.Select((value, index) => $"{index}, {value / (Math.Max(YearGoal, trainingData.Sum) / 100f)}")); // format for svg polyline
        public string SummedPushupsGraphBackground => "0, 0 " + SummedPushupsGraph + " 364, 0";
    }
}