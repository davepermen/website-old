using Home.Services.HttpClients.EvState;
using System;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class ChargeCarInTheMorning : IScheduledTask
    {
        private readonly Client client;

        public TimeSpan? Every => null;
        public TimeSpan? DailyAt => TimeSpan.FromHours(3) + TimeSpan.FromMinutes(15);

        public ChargeCarInTheMorning(Client client)
        {
            this.client = client;
        }

        public async Task Run()
        {
            //var today = DateTime.Today;
            //if(today.DayOfWeek != DayOfWeek.Saturday && today.DayOfWeek != DayOfWeek.Sunday)
            {
                await client.StartCharging(TimeSpan.FromHours(5));
            }
        }
    }
}
