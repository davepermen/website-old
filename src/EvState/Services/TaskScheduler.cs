using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvState.Services
{
    public class TaskScheduler
    {
        List<IScheduledTask> scheduledTasks = new List<IScheduledTask>();

        public TaskScheduler(IEnumerable<IScheduledTask> scheduledTasks)
        {
            this.scheduledTasks.AddRange(scheduledTasks);
        }

        public void Start()
        {
            foreach(var task in scheduledTasks)
            {
                Task.Run(async () =>
                {
                    while(true)
                    {
                        try
                        {
                            await task.InvokeAsync();
                        }
                        catch
                        {
                        }
                        await Task.Delay(task.Every);
                    }
                });
            }
        }
    }
}
