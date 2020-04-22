﻿using Home.Tasks;
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
            var now = DateTime.Now;

            foreach (var scheduledTask in lastTimeRun.Keys.ToArray())
            {
                if (scheduledTask.Every.HasValue)
                {
                    var last = lastTimeRun[scheduledTask];
                    if (now - last >= scheduledTask.Every.Value - TimeSpan.FromSeconds(5)) // a little adjustment for those setting 1 minute tickers, to make sure they run
                    {
                        await scheduledTask.Run();
                        if (lastTimeRun[scheduledTask] != DateTime.MinValue)
                        {
                            lastTimeRun[scheduledTask] += scheduledTask.Every.Value;
                        }
                        else
                        {
                            lastTimeRun[scheduledTask] = now;
                        }
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
                            await scheduledTask.Run();
                            lastTimeRun[scheduledTask] = today;
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
                    }
                }
            }
        }
    }
}
