using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HomeCharging
{
    using System.IO;
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<JsonResult> Toggle()
        {
            if (System.IO.File.Exists("active") == false)
            {
                System.IO.File.WriteAllText("active", "no");
            }

            if (System.IO.File.ReadAllText("active") == "no")
            {
                await Start();
            }
            else
            {
                await Stop();
            }

            return Json(new
            {
                Active = System.IO.File.ReadAllText("active")
            });
        }

        public JsonResult State()
        {
            var recent = Charges.GetRecentCharges();
            if (System.IO.File.Exists("active") == true && System.IO.File.ReadAllText("active") == "yes")
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

            System.IO.File.WriteAllText("active", "yes");
        }

        private async Task StopCharging()
        {
            var id = "e6d1a1fb-c667-42d6-836b-a5704cd87fe8";

            var client = httpClientFactory.CreateClient("ECarUp");
            System.IO.File.WriteAllText("active", "no");
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
                var shouldBeStopped = hasStations == false || System.IO.File.ReadAllText("active") == "no";

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

            System.IO.File.WriteAllText("active", "no");
        }

        public JsonResult Recent() => Json(Charges.GetRecentCharges());

        public JsonResult RefreshLiveTile() { UpdateLiveTile(); return Json("ok"); }

        private void UpdateLiveTile()
        {
            var recent = Charges.GetRecentCharges();
            var template = System.IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{recent}}", Math.Round(recent, 1).ToString());
            System.IO.File.WriteAllText("wwwroot\\livetile.xml", content);
        }
    }
}
