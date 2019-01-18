using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace EvState.HttpClients
{
    public class EVNotifyHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly string akey;
        private readonly string token;
        private readonly string uri;

        public EVNotifyHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.akey = configuration["evnotify-akey"];
            this.token = configuration["evnotify-token"];
            this.uri = configuration["evnotify-uri"];

            this.httpClient = httpClient;
        }

        public async Task<float> GetStateOfCharge()
        {
            var response = await httpClient.GetAsync($"{uri}soc?akey={akey}&token={token}");
            var stateOfCharge = await response.Content.ReadAsAsync<StateOfCharge>();
            return stateOfCharge.soc_display;
        }
    }
}
