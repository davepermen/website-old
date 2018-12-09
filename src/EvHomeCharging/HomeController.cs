using Microsoft.AspNetCore.Mvc;
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
        private readonly string activePath = $@"{Program.DataRoot}\active";

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<JsonResult> Toggle()
        {
            if (IO.File.Exists(activePath) == false)
            {
                IO.File.WriteAllText(activePath, "no");
            }

            if (IO.File.ReadAllText(activePath) == "no")
            {
                await Start();
            }
            else
            {
                await Stop();
            }

            return Json(new
            {
                Active = IO.File.ReadAllText(activePath)
            });
        }

        public JsonResult State()
        {
            var recent = Charges.GetRecentCharges();
            if (IO.File.Exists(activePath) == true && IO.File.ReadAllText(activePath) == "yes")
            {
                var charge = Charges.GetLastCharge();
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
            var chargeTime = TimeSpan.FromHours(1);
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

            IO.File.WriteAllText(activePath, "yes");
        }

        private async Task StopCharging()
        {
            var id = "e6d1a1fb-c667-42d6-836b-a5704cd87fe8";

            var client = httpClientFactory.CreateClient("ECarUp");
            IO.File.WriteAllText(activePath, "no");
            await client.PostAsync("api/DeactivateStation/" + id, null);
        }

        private async Task PollSate(TimeSpan during, TimeSpan every)
        {
            var startTime = DateTime.UtcNow;
            while(startTime < DateTime.UtcNow + during)
            {
                var client = httpClientFactory.CreateClient("ECarUp");

                var response = await client.GetAsync("api/ActiveStations");
                var stations = await response.Content.ReadAsAsync<ChargingState[]>();

                var hasStations = stations.Any();
                var shouldBeStopped = hasStations == false || IO.File.ReadAllText(activePath) == "no";

                if (hasStations)
                {
                    new Charge(stations.First());
                    UpdateLiveTile();
                    await Task.Delay(every);
                }
                
                if(shouldBeStopped)
                {
                    break;
                }
            }

            IO.File.WriteAllText(activePath, "no");
        }

        public JsonResult Recent() => Json(Charges.GetRecentCharges());

        public JsonResult RefreshLiveTile() { UpdateLiveTile(); return Json("ok"); }

        private void UpdateLiveTile()
        {
            var recent = Charges.GetRecentCharges();
            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{recent}}", Math.Round(recent, 1).ToString());
            IO.File.WriteAllText("wwwroot\\livetile.xml", content);
        }
    }
}
