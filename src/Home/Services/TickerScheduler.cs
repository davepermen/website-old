using Conesoft.DataSources;
using Home.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Services
{
    public class TickerScheduler
    {
        private readonly Dictionary<IScheduledTask, DateTime> lastTimeRun;
        private readonly string logPath;

        public string LogPath => logPath;

        public TickerScheduler(IEnumerable<IScheduledTask> scheduledTasks, IDataSources dataSources)
        {
            this.lastTimeRun = new Dictionary<IScheduledTask, DateTime>();

            foreach (var scheduledTask in scheduledTasks)
            {
                lastTimeRun[scheduledTask] = DateTime.MinValue;
            }

            this.logPath = IO.Path.Combine(dataSources.LocalDirectory, "Scheduler", $"{DateTime.Today.ToShortDateString()}.md");

            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(logPath));
        }

        private async Task RunWithLogging(IScheduledTask task)
        {
            try
            {
                await IO.File.AppendAllTextAsync(logPath, $"- **{task.GetType().Name}** *executed* ");
                await task.Run();
            }
            catch(Exception e)
            {
                await IO.File.AppendAllTextAsync(logPath, "*with error* " + e.Message + " (" + e + ")" + Environment.NewLine);
            }
            finally
            {
                await IO.File.AppendAllTextAsync(logPath, "*successfully*" + Environment.NewLine);
            }
        }

        private async Task SkipWithLogging(IScheduledTask task)
        {
            await IO.File.AppendAllTextAsync(logPath, $"- **{task.GetType().Name}** *skipped*" + Environment.NewLine);
        }

        public async Task Tick()
        {
            var now = DateTime.Now;

            await IO.File.AppendAllTextAsync(logPath, $"# {DateTime.Now.ToShortTimeString()}" + Environment.NewLine);

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

            await IO.File.AppendAllTextAsync(logPath, Environment.NewLine + Environment.NewLine);
        }
    }
}
