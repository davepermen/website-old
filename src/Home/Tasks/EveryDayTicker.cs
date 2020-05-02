using Conesoft.DataSources;
using Conesoft.Files;
using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class EveryDayTicker : IScheduledTask
    {
        private readonly File file;

        public TimeSpan? Every => null;
        public TimeSpan? DailyAt => TimeSpan.FromHours(23) + TimeSpan.FromMinutes(11);

        public EveryDayTicker(IDataSources dataSources)
        {
            this.file = dataSources.Local / "Tasks" / File.Name("daily", "txt");
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            await file.AppendLine(now.ToLongDateString() + " " + now.ToLongTimeString());
        }
    }
}
