using Conesoft.DataSources;
using Conesoft.Files;
using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class SimpleTicker : IScheduledTask
    {
        private readonly File file;

        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        public SimpleTicker(IDataSources dataSources)
        {
            this.file = dataSources.Local / "Tasks" / File.Name("tick", "txt");
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            await file.WriteText(now.ToLongDateString() + " " + now.ToLongTimeString());
        }
    }
}
