using Conesoft.DataSources;
using Conesoft.Files;
using Home.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Home.Services
{
    public class TickerScheduler
    {
        private readonly IEnumerable<IScheduledTask> scheduledTasks;
        private readonly Dictionary<IScheduledTask, DateTime> lastTimeRun;

        internal File Logfile { get; }

        public TickerScheduler(IEnumerable<IScheduledTask> scheduledTasks, IDataSources dataSources)
        {
            this.scheduledTasks = scheduledTasks;

            this.lastTimeRun = new Dictionary<IScheduledTask, DateTime>();

            foreach (var scheduledTask in scheduledTasks)
            {
                lastTimeRun[scheduledTask] = DateTime.MinValue;
            }

            this.Logfile = dataSources.Local / "Scheduler" / File.Name(DateTime.Today.ToShortDateString(), "md");
        }

        private async Task RunWithLogging(IScheduledTask task)
        {
            try
            {
                await Logfile.AppendTextAsync($"- **{task.GetType().Name}** *executed* ");
                await task.Run();
            }
            catch (Exception e)
            {
                await Logfile.AppendLineAsync("*with error* " + e.Message + " (" + e + ")");
            }
            finally
            {
                await Logfile.AppendLineAsync("*successfully*");
            }
        }

        internal async Task ForceTick(string taskName)
        {
            await Logfile.AppendLineAsync($"# {DateTime.Now.ToShortTimeString()}");

            var task = scheduledTasks.FirstOrDefault(task => task.GetType().Name.ToLowerInvariant() == taskName.ToLowerInvariant());
            if (task != null)
            {
                await RunWithLogging(task);
            }
        }

        private async Task SkipWithLogging(IScheduledTask task)
        {
            await Logfile.AppendLineAsync($"- **{task.GetType().Name}** *skipped*");
        }

        public async Task Tick()
        {
            var now = DateTime.Now;

            await Logfile.AppendLineAsync($"# {now.ToShortTimeString()}");

            foreach (var scheduledTask in lastTimeRun.Keys.ToArray())
            {
                if (scheduledTask.Every.HasValue)
                {
                    var last = lastTimeRun[scheduledTask];
                    if (now - last >= scheduledTask.Every.Value - TimeSpan.FromSeconds(5)) // a little adjustment for those setting 1 minute tickers, to make sure they run
                    {
                        await RunWithLogging(scheduledTask);
                        if (lastTimeRun[scheduledTask] != DateTime.MinValue)
                        {
                            lastTimeRun[scheduledTask] += scheduledTask.Every.Value;
                        }
                        else
                        {
                            lastTimeRun[scheduledTask] = now;
                        }
                    }
                    else
                    {
                        await SkipWithLogging(scheduledTask);
                    }
                }
                else if (scheduledTask.DailyAt.HasValue)
                {
                    var today = now.Date;
                    var last = lastTimeRun[scheduledTask];

                    var time = now - today;

                    if (lastTimeRun[scheduledTask] != DateTime.MinValue)
                    {
                        if (last < today && time >= scheduledTask.DailyAt.Value - TimeSpan.FromSeconds(5)) // a little adjustment for those setting 1 minute tickers, to make sure they run
                        {
                            await RunWithLogging(scheduledTask);
                            lastTimeRun[scheduledTask] = today;
                        }
                        else
                        {
                            await SkipWithLogging(scheduledTask);
                        }
                    }
                    else
                    {
                        if (time >= scheduledTask.DailyAt.Value - TimeSpan.FromSeconds(5)) // a little adjustment for those setting 1 minute tickers, to make sure they run
                        {
                            lastTimeRun[scheduledTask] = today;
                        }
                        else
                        {
                            lastTimeRun[scheduledTask] = today - TimeSpan.FromDays(1);
                        }
                        await SkipWithLogging(scheduledTask);
                    }
                }
            }

            await Logfile.AppendLineAsync(Environment.NewLine);
        }
    }
}
