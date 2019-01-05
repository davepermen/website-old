using Conesoft;
using System;
using System.Collections.Generic;
using IO = System.IO;

namespace Fitness.Data
{
    public class TrainingData
    {
        private readonly Dictionary<DateTime, int> entries = new Dictionary<DateTime, int>();
        private int[] amountPerDay = new int[0];
        private int[] accumulatedEveryDay = new int[0];
        private int sum = 0;
        private int goal = 0;

        private readonly IDataSources dataSources;
        private readonly string user;
        private readonly string training;
        private readonly int year;

        public TrainingData(IDataSources dataSources, string user, string training, int year)
        {
            this.dataSources = dataSources;
            this.user = user;
            this.training = training;
            this.year = year;

            LoadData(dataSources, user, training, year);
        }

        public void Add(int amount)
        {
            var path = $@"{dataSources.LocalDirectory}\{year}\{user}\{training}";
            var filename = $@"{path}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = IO.File.Exists(filename) ? int.Parse(IO.File.ReadAllText(filename)) : 0;

            IO.Directory.CreateDirectory(path);
            IO.File.WriteAllText(filename, $"{current + amount}");

            LoadData(dataSources, user, training, year);
        }
        
        public int Sum => sum;
        public int Goal => goal;
        public int[] AmountPerDay => amountPerDay;
        public int[] AccumulatedEveryDay => accumulatedEveryDay;

        private void LoadData(IDataSources dataSources, string user, string training, int year)
        {
            var path = $@"{dataSources.LocalDirectory}\{year}\{user}\{training}";
            if (IO.Directory.Exists(path))
            {
                LoadEntries(path);

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
            }
        }

        private void LoadEntries(string path)
        {
            foreach (var file in IO.Directory.GetFiles(path, $"{year}-*.txt"))
            {
                var date = DateTime.Parse(IO.Path.GetFileNameWithoutExtension(file));
                var amount = int.Parse(IO.File.ReadAllText(file));

                entries[date] = entries.TryGetValue(date, out int value) ? value + amount : amount;

                sum += amount;
            }

            if (IO.File.Exists($@"{path}\goal.txt"))
            {
                goal = int.Parse(IO.File.ReadAllText($@"{path}\goal.txt"));
            }
        }

        private int DaysInYear(int year)
        {
            var firstDay = new DateTime(year, 1, 1);
            var firstDayOfNextYear = new DateTime(year + 1, 1, 1);
            return (firstDayOfNextYear - firstDay).Days;
        }
    }
}
