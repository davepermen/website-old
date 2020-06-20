using Home.Data.Tesla;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Home.Services.HttpClients.Tesla
{
    public class Client
    {
        private readonly HttpClient client;
        private readonly IConfigurationSection configuration;

        DateTime lastLogin = DateTime.MinValue;

        public Client(HttpClient client, IConfiguration configuration)
        {
            this.configuration = configuration.GetSection("tesla");

            this.client = client;

            client.BaseAddress = new Uri(@"https://owner-api.teslamotors.com");
            client.DefaultRequestHeaders.Add("X-Tesla-User-Agent", "TeslaApp/3.4.4-350/fad4a582e/android/9.0.0");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 9.0.0; VS985 4G Build/LRX21Y; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/58.0.3029.83 Mobile Safari/537.36");
        }

        async Task Login()
        {
            if (client.DefaultRequestHeaders.Contains("authorization") == false || DateTime.UtcNow - lastLogin > TimeSpan.FromDays(1))
            {
                var authentication = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", configuration["clientid"] },
                    { "client_secret", configuration["secret"] },
                    { "email", configuration["email"] },
                    { "password", configuration["password"] },
                };
                var content = new FormUrlEncodedContent(authentication);

                var response = await client.PostAsync("/oauth/token", content);
                var authenticationToken = await response.Content.ReadFromJsonAsync<Data.Tesla.AuthenticationToken>();
                if (client.DefaultRequestHeaders.Contains("authorization"))
                {
                    client.DefaultRequestHeaders.Remove("authorization");
                }
                client.DefaultRequestHeaders.Add("authorization", $"Bearer {authenticationToken.AccessToken}");
                lastLogin = DateTime.UtcNow;
            }
        }

        public async Task WakeUpCars()
        {
            await Login();

            var vehicles = await client.GetFromJsonAsync<Vehicles>("/api/1/vehicles");

            foreach (var vehicle in vehicles.Response)
            {
                await client.PostAsync($"/api/1/vehicles/{vehicle.Id}/wake_up", null);
            }
        }

        public async Task<Vehicle[]> GetVehicles()
        {
            await Login();

            return (await client.GetFromJsonAsync<Vehicles>("/api/1/vehicles")).Response;
        }

        public async Task<Vehicle?> GetVehicle(string name) => (await GetVehicles()).FirstOrDefault(v => v.DisplayName == name);

        public async Task<State> GetStateOf(Vehicle vehicle)
        {
            return (await client.GetFromJsonAsync<StateResponse>($"/api/1/vehicles/{vehicle.Id}/vehicle_data")).response;
        }
    }
}
