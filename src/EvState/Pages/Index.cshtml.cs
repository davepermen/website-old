using Conesoft;
using EvState.HttpClients;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class IndexModel : PageModel
    {
        public ChargingState[] ChargingState { get; private set; }
        public float StateOfCharge { get; private set; }

        private IDataSources dataSources;

        public async Task OnGet([FromServices] ECarUpHttpClient eCarUpHttpClient, [FromServices] EVNotifyHttpClient evNotifyHttpClient, [FromServices] IDataSources dataSources)
        {
            this.dataSources = dataSources;

            var chargingState = await eCarUpHttpClient.State();

            var stateOfCharge = await evNotifyHttpClient.GetStateOfCharge();

            this.ChargingState = chargingState;

            this.StateOfCharge = stateOfCharge;

            var _ = StartTimer(Request.GetDisplayUrl());
        }

        private async Task StartTimer(string uri)
        {
            System.IO.File.WriteAllText($"{dataSources.LocalDirectory}/{DateTime.Now.ToShortTimeString().Replace(":", ".")}.txt", "");
            await Task.Delay(TimeSpan.FromMinutes(1));
            using (var client = new HttpClient())
            {
                await client.GetAsync(uri);
            }
        }
    }
}
