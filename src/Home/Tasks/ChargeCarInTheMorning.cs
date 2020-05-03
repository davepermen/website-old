using EvState = Home.Services.HttpClients.EvState;
using Tesla = Home.Services.HttpClients.Tesla;
using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class ChargeCarInTheMorning : IScheduledTask
    {
        private readonly EvState.Client evState;
        private readonly Tesla.Client tesla;

        public TimeSpan? Every => null;
        public TimeSpan? DailyAt => TimeSpan.FromHours(3) + TimeSpan.FromMinutes(53);

        public ChargeCarInTheMorning(EvState.Client evState, Tesla.Client tesla)
        {
            this.evState = evState;
            this.tesla = tesla;
        }

        public async Task Run()
        {
            //var today = DateTime.Today;
            //if(today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday)
            {
                await tesla.WakeUpCars();
                await evState.StartCharging(TimeSpan.FromHours(5));
            }
        }
    }
}
