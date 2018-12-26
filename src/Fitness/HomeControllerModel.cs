using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Fitness
{
    public class HomeControllerModel
    {
        readonly List<int> dailyPushups = new List<int>();
        readonly List<int> summedPushups = new List<int>();

        public HomeControllerModel(string training, string user, Conesoft.IDataSources dataSources)
        {
            var path = $@"{dataSources.LocalDirectory}\{training}\{user}";
            if(Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*.txt"))
                {
                    var pushup = int.Parse(File.ReadAllText(file));
                    dailyPushups.Add(pushup);
                    Pushups += pushup;
                    summedPushups.Add(Pushups);
                }
            }
        }

        public string Me { get; set; }

        public int Pushups { get; } = 0;

        public string DailyPushupsGraph => string.Join(" ", dailyPushups.Select((value, index) => $"{index}, {value * .5f}")); // format for svg polyline
        public string DailyPushupsGraphBackground => DailyPushupsGraph + " 364, 0";
        public string SummedPushupsGraph => string.Join(" ", summedPushups.Select((value, index) => $"{index}, {value * .02f}")); // format for svg polyline
        public string SummedPushupsGraphBackground => SummedPushupsGraph + " 364, 0";
    }
}