using Conesoft.DataSources;
using Home.Data.FoldingAtHome.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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

        public FoldingAtHomeReader(IDataSources dataSources, IConfiguration configuration, IHttpClientFactory factory)
        {
            this.client = factory.CreateClient("folding@home");
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
            var content = await client.GetFromJsonAsync<Data.FoldingAtHome.Server.Response>("https://stats.foldingathome.org/api/donor/davepermen");

            var team = content.Teams.FirstOrDefault(team => team.Name.StartsWith("LinusTechTips"));
            if (team != null)
            {
                var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Folding@Home", "ServerStats.txt");
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
                await IO.File.WriteAllTextAsync(path, team.WorkUnits + Environment.NewLine + team.Credit);
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

            var slots = BrokenJsonSerializer.DeserializeFromBrokenJson(text).Where(slot => slot.Status == SlotStatus.Running);

            var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Folding@Home", "ClientStatus.txt");

            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));

            using var stream = IO.File.Create(path);
            await JsonSerializer.SerializeAsync(stream, slots, new JsonSerializerOptions { WriteIndented = true });
            stream.Close();
        }
    }
}
