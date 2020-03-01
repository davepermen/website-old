using Home.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Home.Services
{
    public class TickerScheduler
    {
        private readonly Dictionary<IScheduledTask, DateTime> lastTimeRun;

        public TickerScheduler(IEnumerable<IScheduledTask> scheduledTasks)
        {
            this.lastTimeRun = new Dictionary<IScheduledTask, DateTime>();

            foreach (var scheduledTask in scheduledTasks)
            {
                lastTimeRun[scheduledTask] = DateTime.MinValue;
            }
        }

        public async Task Tick()
        {
            var now = DateTime.UtcNow;

            foreach (var scheduledTask in lastTimeRun.Keys.ToArray())
            {
                var last = lastTimeRun[scheduledTask];
                if (now - last >= scheduledTask.Every - TimeSpan.FromSeconds(5)) // a little adjustment for those setting 1 minute tickers, to make sure they run
                {
                    await scheduledTask.Run();
                    if (lastTimeRun[scheduledTask] != DateTime.MinValue)
                    {
                        lastTimeRun[scheduledTask] += scheduledTask.Every;
                    }
                    else
                    {
                        lastTimeRun[scheduledTask] = now;
                    }
                }
            }
        }
    }
}
