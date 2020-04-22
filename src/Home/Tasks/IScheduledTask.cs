using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public interface IScheduledTask
    {
        TimeSpan? Every { get; }
        TimeSpan? DailyAt { get; }

        Task Run();
    }
}
