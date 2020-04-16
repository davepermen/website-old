using Conesoft.DataSources;
using System;
using System.Collections.Generic;
using IO = System.IO;

namespace Fitness.Data
{
    public class TrainingData
    {
        private readonly IDataSources dataSources;

        private readonly Dictionary<DateTime, int> entries = new Dictionary<DateTime, int>();

        public int Sum { get; private set; } = 0;
        public int Goal { get; private set; } = 0;
        public int[] AmountPerDay { get; private set; } = new int[0];
        public int[] AccumulatedEveryDay { get; private set; } = new int[0];
        public string User { get; }
        public string Training { get; }
        public int Year { get; }

        public TrainingData(IDataSources dataSources, string user, string training, int year)
        {
            this.dataSources = dataSources;
            this.User = user;
            this.Training = training;
            this.Year = year;

            LoadData(dataSources, user, training, year);
        }

        public void Add(int amount)
        {
            var path = $@"{dataSources.LocalDirectory}\{Year}\{User}\{Training}";
            var filename = $@"{path}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = IO.File.Exists(filename) ? int.Parse(IO.File.ReadAllText(filename)) : 0;

            IO.Directory.CreateDirectory(path);
            IO.File.WriteAllText(filename, $"{current + amount}");

            LoadData(dataSources, User, Training, Year);
        }

        private void LoadData(IDataSources dataSources, string user, string training, int year)
        {
            var path = $@"{dataSources.LocalDirectory}\{year}\{user}\{training}";
            if (IO.Directory.Exists(path))
            {
                LoadEntries(path);

                AmountPerDay = new int[DaysInYear(year)];
                AccumulatedEveryDay = new int[DaysInYear(year)];

                foreach (var date in entries.Keys)
                {
                    AmountPerDay[date.DayOfYear - 1] = entries[date];
                }

                var accumulated = 0;
                for (var i = 0; i < AmountPerDay.Length; i++)
                {
                    accumulated += AmountPerDay[i];
                    AccumulatedEveryDay[i] = accumulated;
                }
            }
        }

        private void LoadEntries(string path)
        {
            foreach (var file in IO.Directory.GetFiles(path, $"{Year}-*.txt"))
            {
                var date = DateTime.Parse(IO.Path.GetFileNameWithoutExtension(file));
                var amount = int.Parse(IO.File.ReadAllText(file));

                entries[date] = entries.TryGetValue(date, out int value) ? value + amount : amount;

                Sum += amount;
            }

            if (IO.File.Exists($@"{path}\goal.txt"))
            {
                Goal = int.Parse(IO.File.ReadAllText($@"{path}\goal.txt"));
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
