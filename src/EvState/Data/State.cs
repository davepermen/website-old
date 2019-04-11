using Conesoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvState.Data
{
    public class State
    {
        private readonly IDataSources dataSources;
        private ToSerialize state;

        public State(IDataSources dataSources)
        {
            this.dataSources = dataSources;
            this.state = default(ToSerialize);
            var _ = Load();
        }

        public void SetChargeOfCar(float kiloWatts)
        {
            this.state.ChargeOfCar = (kiloWatts, DateTime.Now);
            var _ = Save();
        }

        public void SetChargeOfWallbox(float kiloWatts)
        {
            this.state.ChargeOfWallbox = (kiloWatts, DateTime.Now);
            var _ = Save();
        }

        async Task Load()
        {
            var file = $@"{dataSources.LocalDirectory}\state.json";
            if (File.Exists(file))
            {
                var content = await File.ReadAllTextAsync(file);
                this.state = JsonConvert.DeserializeObject<ToSerialize>(content);
            }
            else
            {
                this.state = new ToSerialize();
            }
        }

        async Task Save()
        {
            var file = $@"{dataSources.LocalDirectory}\state.json";
            await File.WriteAllTextAsync(file, JsonConvert.SerializeObject(this.state, Formatting.Indented));
        }

        class ToSerialize
        {
            public (float kiloWatts, DateTime setAt) ChargeOfCar { get; set; }
            public (float kiloWatts, DateTime setAt) ChargeOfWallbox { get; set; }
        }
    }
}
