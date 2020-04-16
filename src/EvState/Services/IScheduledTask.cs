using System;
using System.Threading.Tasks;

namespace EvState.Services
{
    public interface IScheduledTask
    {
        public TimeSpan Every { get; }
        public Task InvokeAsync();
    }
}