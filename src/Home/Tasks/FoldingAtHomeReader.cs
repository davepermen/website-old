using Conesoft.DataSources;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class FoldingAtHomeReader : IScheduledTask
    {
        private readonly HttpClient client;
        private readonly IDataSources dataSources;

        public TimeSpan Every => TimeSpan.FromMinutes(10);

        public FoldingAtHomeReader(IDataSources dataSources)
        {
            this.client = new HttpClient();
            this.dataSources = dataSources;
        }

        public async Task Run()
        {
            var result = await client.GetAsync("https://stats.foldingathome.org/api/donor/davepermen");
            var stream = await result.Content.ReadAsStreamAsync();
            var content = await JsonSerializer.DeserializeAsync<Rootobject>(stream);
            var team = content.teams.FirstOrDefault(team => team.name.StartsWith("LinusTechTips"));
            if(team != null)
            {
                var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Folding@Home", "Status.txt");
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
                await IO.File.WriteAllTextAsync(path, team.wus + Environment.NewLine + team.credit);
            }
        }


        public class Rootobject
        {
            public int wus { get; set; }
            public int rank { get; set; }
            public int total_users { get; set; }
            public int active_50 { get; set; }
            public string path { get; set; }
            public string wus_cert { get; set; }
            public int id { get; set; }
            public string credit_cert { get; set; }
            public string last { get; set; }
            public string name { get; set; }
            public Team[] teams { get; set; }
            public int active_7 { get; set; }
            public int credit { get; set; }
        }

        public class Team
        {
            public int wus { get; set; }
            public int uid { get; set; }
            public int active_50 { get; set; }
            public int active_7 { get; set; }
            public int credit { get; set; }
            public int team { get; set; }
            public string name { get; set; }
            public string last { get; set; }
        }

    }
}
