using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public interface IScheduledTask
    {
        TimeSpan Every { get; }

        Task Run();
    }
}
