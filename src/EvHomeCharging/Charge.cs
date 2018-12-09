namespace EvHomeCharging
{
    using System;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class Charge
    {
        private static readonly string log = $@"{Program.DataRoot}\log";
        private static readonly string charges = $@"{Program.DataRoot}\charges";

        public Charge(ChargingState state)
        {
            Directory.CreateDirectory(charges);
            var files = Directory.GetFiles(charges, "*.txt");
            var counter = files.Max(f => (int?)int.Parse(Path.GetFileNameWithoutExtension(f))) ?? 0;

            var currentCharge = File.Exists($"{charges}\\{counter}.txt") ? float.Parse(File.ReadAllText($"{charges}\\{counter}.txt")) : float.NegativeInfinity;

            if(state.EnergyUsed > currentCharge)
            {
                File.WriteAllText($"{charges}\\{counter}.txt", state.EnergyUsed.ToString());
            }
            else if(state.EnergyUsed < currentCharge)
            {
                File.WriteAllText($"{charges}\\{counter + 1}.txt", state.EnergyUsed.ToString());
            }

            Directory.CreateDirectory(log);
            File.WriteAllText($"{log}\\{DateTime.UtcNow.ToString("s").Replace(":", "-")}.json", JsonConvert.SerializeObject(state, Formatting.Indented));
        }
    }
}