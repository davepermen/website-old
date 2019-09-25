using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using IO = System.IO;

namespace EvHomeCharging
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IDataSources dataSources;
        private readonly IConfiguration configuration;
        private readonly Charges charges;

        public HomeController(IHttpClientFactory httpClientFactory, IDataSources dataSources, IConfiguration configuration, Charges charges)
        {
            this.httpClientFactory = httpClientFactory;
            this.dataSources = dataSources;
            this.configuration = configuration;
            this.charges = charges;
        }

        private string ActivePath => $@"{dataSources.LocalDirectory}\active";

        public async Task<JsonResult> Toggle()
        {
            if (IO.File.Exists(ActivePath) == false)
            {
                IO.File.WriteAllText(ActivePath, "no");
            }

            if (IO.File.ReadAllText(ActivePath) == "no")
            {
                await Start();
            }
            else
            {
                await Stop();
            }

            return Json(new
            {
                Active = IO.File.ReadAllText(ActivePath)
            });
        }

        public async Task<JsonResult> State()
        {
            var recent = charges.GetRecentCharges();
            if (IO.File.Exists(ActivePath) == true && IO.File.ReadAllText(ActivePath) == "yes")
            {
                var charge = charges.GetLastCharge();
                await UpdateLiveTile();
                return Json(new
                {
                    Current = charge,
                    Recent = recent
                });
            }
            return Json(new
            {
                Current = new ChargingState(),
                Recent = recent
            });
        }

        public async Task<JsonResult> Start()
        {
            var chargeTime = TimeSpan.FromHours(5);
            await StartCharging(chargeTime);
            var _ = PollSate(
                during: chargeTime,
                every: TimeSpan.FromSeconds(5)
            );
            return Json("ok");
        }

        public async Task Stop()
        {
            await StopCharging();
        }

        private async Task StartCharging(TimeSpan during)
        {
            var id = "e6d1a1fb-c667-42d6-836b-a5704cd87fe8";
            var seconds = (int)during.TotalSeconds;

            var client = httpClientFactory.CreateClient("ECarUp");
            await client.PostAsync("api/ActivateStation/" + id + "?seconds=" + seconds, null);

            IO.File.WriteAllText(ActivePath, "yes");
        }

        private async Task StopCharging()
        {
            var id = "e6d1a1fb-c667-42d6-836b-a5704cd87fe8";

            var client = httpClientFactory.CreateClient("ECarUp");
            IO.File.WriteAllText(ActivePath, "no");
            await client.PostAsync("api/DeactivateStation/" + id, null);
        }

        private async Task PollSate(TimeSpan during, TimeSpan every)
        {
            var startTime = DateTime.UtcNow;
            while (startTime < DateTime.UtcNow + during)
            {
                var client = httpClientFactory.CreateClient("ECarUp");

                var response = await client.GetAsync("api/ActiveStations");
                var stations = await response.Content.ReadAsAsync<ChargingState[]>();

                var hasStations = stations.Any();
                var shouldBeStopped = hasStations == false || IO.File.ReadAllText(ActivePath) == "no";

                if (hasStations)
                {
                    charges.Add(stations.First());
                    await UpdateLiveTile();
                    await Task.Delay(every);
                }

                if (shouldBeStopped)
                {
                    break;
                }
            }

            IO.File.WriteAllText(ActivePath, "no");
        }

        public JsonResult Recent() => Json(charges.GetRecentCharges());

        public async Task<JsonResult> RefreshLiveTile() { await UpdateLiveTile(); return Json("ok"); }

        private async Task UpdateLiveTile()
        {
            var recent = charges.GetRecentCharges();
            var soc = await GetSoC();
            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{recent}}", Math.Round(recent, 1).ToString()).Replace("{{soc}}", Math.Round(soc, 1).ToString());
            IO.File.WriteAllText("wwwroot\\livetile.xml", content);
        }

        private async Task<float> GetSoC()
        {
            var lastSocFilePath = $"{dataSources.LocalDirectory}/last_soc.txt";
            var socFilePath = $"{dataSources.LocalDirectory}/soc.txt";
            var lastRequest = IO.File.Exists(lastSocFilePath) ? DateTime.Parse(IO.File.ReadAllText(lastSocFilePath)) : DateTime.MinValue;

            var now = DateTime.Now;
            if ((now - lastRequest).TotalMinutes > 1)
            {
                IO.File.WriteAllText(lastSocFilePath, now.ToString());

                var akey = configuration["evnotify-akey"];
                var token = configuration["evnotify-token"];
                var client = new HttpClient();

                var response = await client.GetAsync($"https://app.evnotify.de/soc?akey={akey}&token={token}");
                var result = await response.Content.ReadAsStringAsync();
                var soc = Newtonsoft.Json.JsonConvert.DeserializeObject<SoC>(result).soc_display;

                IO.File.WriteAllText(socFilePath, soc.ToString());

                return soc;
            }
            else
            {
                return IO.File.Exists(socFilePath) ? float.Parse(IO.File.ReadAllText(socFilePath)) : 0;
            }
        }

        class SoC
        {
            public float soc_display { get; set; }
        }
    }
}
