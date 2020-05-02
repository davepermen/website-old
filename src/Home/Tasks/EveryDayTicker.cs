using Conesoft.DataSources;
using System;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class EveryDayTicker : IScheduledTask
    {
        private readonly IDataSources dataSources;

        public TimeSpan? Every => null;
        public TimeSpan? DailyAt => TimeSpan.FromHours(23) + TimeSpan.FromMinutes(11);

        public EveryDayTicker(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            var path = IO.Path.Combine(dataSources.LocalDirectory, "Tasks", "daily.txt");
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
            await IO.File.AppendAllTextAsync(path, now.ToLongDateString() + " " + now.ToLongTimeString());
        }
    }
}
