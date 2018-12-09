namespace EvHomeCharging
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;

    public class Charges
    {
        private static readonly string log = $@"{Program.DataRoot}\log";

        public struct ChargeAt
        {
            public float Charge { get; set; }
            public DateTime At { get; set; }
        }

        public static float GetRecentCharges(int recentHours = 18)
        {
            if (Directory.Exists(log))
            {
                var files = Directory.GetFiles(log, "*.json").Select(f => new FileInfo(f));

                var recentFiles = files.Where(f => (DateTime.UtcNow - f.LastWriteTimeUtc).TotalHours < recentHours);

                var states = recentFiles.Select(f => JsonConvert.DeserializeObject<ChargingState>(File.ReadAllText(f.FullName)));

                var sum = 0f;
                var last = 0f;
                foreach(var i in states)
                {
                    if(i.EnergyUsed > last)
                    {
                        last = i.EnergyUsed;
                    }
                    else if(i.EnergyUsed < last)
                    {
                        sum += last;
                        last = 0;
                    }
                }

                return sum;
            }
            else
            {
                return 0;
            }
        }

        public static ChargingState GetLastCharge()
        {
            if (Directory.Exists(log))
            {
                var files = Directory.GetFiles(log, "*.json").Select(f => new FileInfo(f));

                var mostRecentFile = files.OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault();

                return JsonConvert.DeserializeObject<ChargingState>(File.ReadAllText(mostRecentFile.FullName));
            }
            else
            {
                return new ChargingState();
            }
        }
    }
}