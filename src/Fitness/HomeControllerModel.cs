using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fitness
{
    public class HomeControllerModel
    {
        int allpushups = 0;
        List<int> dailyPushups = new List<int>();
        List<int> summedPushups = new List<int>();
        public HomeControllerModel(string db)
        {
            foreach(var file in Directory.GetFiles(db, "*.txt"))
            {
                var pushup = int.Parse(File.ReadAllText(file));
                dailyPushups.Add(pushup);
                allpushups += pushup;
                summedPushups.Add(allpushups);
            }
        }

        public string Me { get; set; }

        public int Pushups => allpushups;

        public string DailyPushupsGraph => string.Join(" ", dailyPushups.Select((value, index) => $"{index}, {value * .5f}")); // format for svg polyline
        public string DailyPushupsGraphBackground => DailyPushupsGraph + " 364, 0";
        public string SummedPushupsGraph => string.Join(" ", summedPushups.Select((value, index) => $"{index}, {value * .02f}")); // format for svg polyline
        public string SummedPushupsGraphBackground => SummedPushupsGraph + " 364, 0";
    }
}