using Conesoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            await WakeUpTesla();

            IO.File.WriteAllText(ActivePath, "yes");
        }

        private async Task WakeUpTesla()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("X-Tesla-User-Agent", configuration["tesla-x-user-agent"]);
            httpClient.DefaultRequestHeaders.Add("User-Agent", configuration["tesla-user-agent"]);

            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("client_id", configuration["tesla-client-id"]),
                    new KeyValuePair<string, string>("client_secret", configuration["tesla-client-secret"]),
                    new KeyValuePair<string, string>("email", configuration["tesla-username"]),
                    new KeyValuePair<string, string>("password", configuration["tesla-password"])
            });
            var authenticationToken = await httpClient.PostAsync<TeslaAuthenticationToken>(configuration["tesla-base-uri"] + "/oauth/token", content);

            httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {authenticationToken.access_token}");

            var vehicles = await httpClient.GetAsync<TeslaVehicles>(configuration["tesla-base-uri"] + "/api/1/vehicles");

            foreach (var vehicle in vehicles.response)
            {
                await httpClient.PostAsync(configuration["tesla-base-uri"] + $"/api/1/vehicles/{vehicle.id}/wake_up", null);
            }
        }

        private class TeslaAuthenticationToken
        {
            public string access_token { get; set; }
        }

        private class TeslaVehicles
        {
            public TeslaVehicle[] response { get; set; }
        }

        private class TeslaVehicle
        {
            public long id { get; set; }
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
            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{recent}}", Math.Round(recent, 1).ToString());
            IO.File.WriteAllText("wwwroot\\livetile.xml", content);

            await Task.CompletedTask;
        }
    }
    public static class PostHelper
    {
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string requestUri, HttpContent content)
        {
            var result = await httpClient.PostAsync(requestUri, content);
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri)
        {
            var result = await httpClient.GetAsync(requestUri);
            return JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
        }
    }
}
