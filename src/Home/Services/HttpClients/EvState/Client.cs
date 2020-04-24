using Home.Data.ECarUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Home.Services.HttpClients.EvState
{
    public class Client
    {
        private readonly HttpClient client;
        private readonly string id;

        public Client(HttpClient httpClient, IConfiguration configuration)
        {
            var section = configuration.GetSection("ecarup");
            var username = section["username"];
            var password = section["password"];
            var uri = section["uri"];
            var id = section["id"];

            this.client = httpClient;
            this.id = id;

            httpClient.BaseAddress = new Uri(uri);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password)));
        }

        public async Task StartCharging(TimeSpan during)
        {
            var seconds = (int)during.TotalSeconds;
            await client.PostAsync("api/ActivateStation/" + id + "?seconds=" + seconds, null);
        }

        public async Task WaitTillCharging()
        {
            while ((await State()) == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }

        public async Task WaitTillNotCharging()
        {
            while ((await State()) != null)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.5));
            }
        }

        public async Task StopCharging() => await client.PostAsync("api/DeactivateStation/" + id, null);

        public Task<ChargingState[]> ActiveStations() => client.GetFromJsonAsync<ChargingState[]>("api/ActiveStations");

        public async Task<ChargingState> State() => (await ActiveStations()).FirstOrDefault();

        public Task<History[]> GetHistory() => client.GetFromJsonAsync<History[]>("api/DriverHistory?startDate=2015-12-28T15%3A12%3A35.023Z");
    }
}