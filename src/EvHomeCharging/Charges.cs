namespace EvHomeCharging
{
    using Conesoft;
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;

    public class Charges
    {
        private readonly IDataSources dataSources;

        private string LogDirectory => $@"{dataSources.LocalDirectory}\log";
        private string ChargesDirectory => $@"{dataSources.LocalDirectory}\charges";

        public Charges(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        public struct ChargeAt
        {
            public float Charge { get; set; }
            public DateTime At { get; set; }
        }

        public float GetRecentCharges(int recentHours = 18)
        {
            if (Directory.Exists(LogDirectory))
            {
                var today = DateTime.UtcNow.ToString("s").Replace(":", "-").Substring(0, 4 + 1 + 2 + 1 + 2);
                var yesterday = DateTime.UtcNow.AddDays(-1).ToString("s").Replace(":", "-").Substring(0, 4 + 1 + 2 + 1 + 2);

                var filesOfToday = Directory.GetFiles(LogDirectory, $"{today}*.json").Select(f => new FileInfo(f));
                var filesOfYesterday = Directory.GetFiles(LogDirectory, $"{yesterday}*.json").Select(f => new FileInfo(f));

                var files = filesOfToday.Concat(filesOfYesterday);

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

        public ChargingState GetLastCharge()
        {
            if (Directory.Exists(LogDirectory))
            {
                var files = Directory.GetFiles(LogDirectory, "*.json").Select(f => new FileInfo(f));

                var mostRecentFile = files.OrderByDescending(f => f.LastWriteTimeUtc).FirstOrDefault();

                return JsonConvert.DeserializeObject<ChargingState>(File.ReadAllText(mostRecentFile.FullName));
            }
            else
            {
                return new ChargingState();
            }
        }

        public void Add(ChargingState chargingState)
        {
            Directory.CreateDirectory(ChargesDirectory);
            var files = Directory.GetFiles(ChargesDirectory, "*.txt");
            var counter = files.Max(f => (int?)int.Parse(Path.GetFileNameWithoutExtension(f))) ?? 0;

            var currentCharge = File.Exists($"{ChargesDirectory}\\{counter}.txt") ? float.Parse(File.ReadAllText($"{ChargesDirectory}\\{counter}.txt")) : float.NegativeInfinity;

            if (chargingState.EnergyUsed > currentCharge)
            {
                File.WriteAllText($"{ChargesDirectory}\\{counter}.txt", chargingState.EnergyUsed.ToString());
            }
            else if (chargingState.EnergyUsed < currentCharge)
            {
                File.WriteAllText($"{ChargesDirectory}\\{counter + 1}.txt", chargingState.EnergyUsed.ToString());
            }

            Directory.CreateDirectory(LogDirectory);
            File.WriteAllText($"{LogDirectory}\\{DateTime.UtcNow.ToString("s").Replace(":", "-")}.json", JsonConvert.SerializeObject(chargingState, Formatting.Indented));
        }
    }
}