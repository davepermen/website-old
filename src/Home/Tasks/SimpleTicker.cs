using Conesoft.DataSources;
using System;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class SimpleTicker : IScheduledTask
    {
        private readonly IDataSources dataSources;

        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        public SimpleTicker(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            var path = IO.Path.Combine(dataSources.LocalDirectory, "Tasks", "tick.txt");
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
            await IO.File.WriteAllTextAsync(path, now.ToLongDateString() + " " + now.ToLongTimeString());
        }
    }
}
