using Conesoft.DataSources;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class FoldingAtHomeReader : IScheduledTask
    {
        private readonly HttpClient client;
        private readonly IDataSources dataSources;
        private readonly IConfigurationSection configuration;

        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        public FoldingAtHomeReader(IDataSources dataSources, IConfiguration configuration)
        {
            this.client = new HttpClient();
            this.dataSources = dataSources;
            this.configuration = configuration.GetSection("folding@home");
        }

        public async Task Run()
        {
            await CheckServerStats();
            await CheckClientStatus();
        }

        async Task CheckServerStats()
        {
            var result = await client.GetAsync("https://stats.foldingathome.org/api/donor/davepermen");
            var stream = await result.Content.ReadAsStreamAsync();
            var content = await JsonSerializer.DeserializeAsync<ServerStatsData.Rootobject>(stream);
            var team = content.teams.FirstOrDefault(team => team.name.StartsWith("LinusTechTips"));
            if (team != null)
            {
                var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Folding@Home", "ServerStats.txt");
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
                await IO.File.WriteAllTextAsync(path, team.wus + Environment.NewLine + team.credit);
            }
        }

        async Task<string> GetSessionId()
        {
            var foldingAtHome = new
            {
                client = configuration["client"],
                port = configuration["port"]
            };

            await client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}");

            var result = await client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/js/main.js");
            var text = await result.Content.ReadAsStringAsync();

            /* i mean, really.. but whatever */

            var sid = text.Substring(0, text.IndexOf(";")).Replace("sid = '", "").Replace("'", "");

            return sid;
        }

        async Task CheckClientStatus()
        {
            /** when you need to document, with comments, a web api interface, that interface is just wrong
                read out session id          http://192.168.1.10:7396/js/main.js
                request and let open:        http://192.168.1.10:7396/api/updates/set?sid={sid}&update_id=0&update_rate=1&update_path=%2Fapi%2Fbasic&_=1587320338580
                request and let open:        http://192.168.1.10:7396/api/updates/set?sid={sid}&update_id=1&update_rate=1&update_path=%2Fapi%2Fslots&_=1587320338581
                request and let open:        http://192.168.1.10:7396/api/configured?sid={sid}&_=1587320338582
                request once for wrong data: http://192.168.1.10:7396/api/updates?sid={sid}&_=1587320338583
                request actual data:         http://192.168.1.10:7396/api/updates?sid={sid}&_=1587320338584
            **/

            var foldingAtHome = new
            {
                client = configuration["client"],
                port = configuration["port"],
                sid = await GetSessionId()
            };

            var text = "";
            try
            {
                var ctx = new CancellationTokenSource();

                var step1 = client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/api/updates/set?sid={foldingAtHome.sid}&update_id=0&update_rate=1&update_path=%2Fapi%2Fbasic&_=1587320338580", ctx.Token);
                await Task.Delay(10);
                var step2 = client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/api/updates/set?sid={foldingAtHome.sid}&update_id=1&update_rate=1&update_path=%2Fapi%2Fslots&_=1587320338581", ctx.Token);
                await Task.Delay(10);
                var step3 = client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/api/configured?sid={foldingAtHome.sid}&_=1587320338582", ctx.Token);
                await Task.Delay(10);
                var step4 = await client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/api/updates?sid={foldingAtHome.sid}&_=1587320338583");

                var result = await client.GetAsync($"http://{foldingAtHome.client}:{foldingAtHome.port}/api/updates?sid={foldingAtHome.sid}&_=1587320338584");
                text = await result.Content.ReadAsStringAsync();

                ctx.Cancel();
            }
            catch (OperationCanceledException)
            {

            }

            var slots = ClientStatusData.Slots.FromJson(text);

            var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Folding@Home", "ClientStatus.txt");

            if (slots.slots.Any(slot => slot.status == "RUNNING"))
            {
                var slot = slots.slots.First(slot => slot.status == "RUNNING");
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
                await IO.File.WriteAllTextAsync(path, slot.percentdone + Environment.NewLine + slot.eta);
            } else
            {
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
                await IO.File.WriteAllTextAsync(path, "");
            }
        }
    }

    namespace ServerStatsData
    {
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

    namespace ClientStatusData
    {
        public class Slots /* modified, not original json result */
            {
            public Slot[] slots { get; set; }

            public static Slots FromJson(string jsonContent)
            {
                var text = jsonContent;
                text = text.Substring(text.IndexOf(",") + 1);
                text = text.Substring(0, text.LastIndexOf("]"));
                text = text.Substring(0, text.LastIndexOf("]"));
                var objectParts = "{ \"slots\": " + text + "}";
                return JsonSerializer.Deserialize<ClientStatusData.Slots>(objectParts);
            }
        }

        public class Slot
        {
            public string id { get; set; }
            public string status { get; set; }
            public string description { get; set; }
            public Options options { get; set; }
            public string reason { get; set; }
            public bool idle { get; set; }
            public int unit_id { get; set; }
            public int project { get; set; }
            public int run { get; set; }
            public int clone { get; set; }
            public int gen { get; set; }
            public string percentdone { get; set; }
            public string eta { get; set; }
            public string ppd { get; set; }
            public string creditestimate { get; set; }
            public string waitingon { get; set; }
            public string nextattempt { get; set; }
            public string timeremaining { get; set; }
        }

        public class Options
        {
            public string paused { get; set; }
        }
    }
}
