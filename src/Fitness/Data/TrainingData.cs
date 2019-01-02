using Conesoft;
using System;
using System.Collections.Generic;
using System.Linq;
using IO = System.IO;

namespace Fitness.Data
{
    public class TrainingData
    {
        private readonly Dictionary<DateTime, int> entries = new Dictionary<DateTime, int>();
        private readonly int[] amountPerDay;
        private readonly int[] accumulatedEveryDay;
        private readonly int sum;
        private readonly int goal;

        public TrainingData(IDataSources dataSources, string user, string training, int year)
        {
            var path = $@"{dataSources.LocalDirectory}\{year}\{user}\{training}";
            if (IO.Directory.Exists(path))
            {
                foreach (var file in IO.Directory.GetFiles(path, $"{year}-*.txt"))
                {
                    var date = DateTime.Parse(IO.Path.GetFileNameWithoutExtension(file));
                    var amount = int.Parse(IO.File.ReadAllText(file));

                    if (entries.ContainsKey(date))
                    {
                        entries[date] += amount;
                    }
                    else
                    {
                        entries[date] = amount;
                    }

                    sum += amount;
                }

                amountPerDay = new int[DaysInYear(year)];
                accumulatedEveryDay = new int[DaysInYear(year)];

                foreach (var date in entries.Keys)
                {
                    amountPerDay[date.DayOfYear - 1] = entries[date];
                }

                var accumulated = 0;
                for (var i = 0; i < amountPerDay.Length; i++)
                {
                    accumulated += amountPerDay[i];
                    accumulatedEveryDay[i] = accumulated;
                }

                if (IO.File.Exists($@"{path}\goal.txt"))
                {
                    goal = int.Parse(IO.File.ReadAllText($@"{path}\goal.txt"));
                }
            }
            else
            {
                amountPerDay = new int[0];
                accumulatedEveryDay = new int[0];
                sum = 0;
                goal = 0;
            }
        }

        private int DaysInYear(int year)
        {
            var firstDay = new DateTime(year, 1, 1);
            var firstDayOfNextYear = new DateTime(year + 1, 1, 1);
            return (firstDayOfNextYear - firstDay).Days;
        }

        public Dictionary<DateTime, int> Counter => entries;
        public int Sum => sum;
        public int Goal => goal;
        public int[] AmountPerDay => amountPerDay;
        public int[] AccumulatedEveryDay => accumulatedEveryDay;
    }
}
