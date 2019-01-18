using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EvState.HttpClients
{
    public class ECarUpHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly string id;

        public ECarUpHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            var username = configuration["ecarup-username"];
            var password = configuration["ecarup-password"];
            var uri = configuration["ecarup-uri"];
            var id = configuration["ecarup-id"];

            this.httpClient = httpClient;
            this.id = id;

            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password)));
        }

        public async Task StartCharging(TimeSpan during)
        {
            var seconds = (int)during.TotalSeconds;
            await httpClient.PostAsync("api/ActivateStation/" + id + "?seconds=" + seconds, null);
        }

        public async Task StopCharging()
        {
            await httpClient.PostAsync("api/DeactivateStation/" + id, null);
        }

        public async Task<ChargingState[]> State()
        {
            var response = await httpClient.GetAsync("api/ActiveStations");
            return await response.Content.ReadAsAsync<ChargingState[]>();
        }
    }
}