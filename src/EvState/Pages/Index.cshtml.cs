using EvState.HttpClients;
using EvState.HttpClients.ECarUp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace EvState.Pages
{
    public class IndexModel : PageModel
    {
        public ChargingState[] ChargingState { get; private set; }
        public History[] History { get; private set; }
        public (DateTime day, double cost, double charge)[] CostPerDay { get; private set; }
        public (DateTime week, double cost, double charge)[] CostPerWeek { get; private set; }

        public async Task OnGet([FromServices] ECarUpHttpClient eCarUpHttpClient)
        {
            ChargingState = await eCarUpHttpClient.State();
            History = await eCarUpHttpClient.GetHistory();

            CostPerDay = History
                .GroupBy(h => h.DateTime.Date)
                .OrderByDescending(h => h.Key)
                .Select(h =>
                (
                    day: h.Key,
                    cost: h.Sum(e => e.PriceConsumption),
                    charge: h.Sum(e => e.Consumption)
                ))
                .ToArray();

            CostPerWeek = History
                .GroupBy(h => h.DateTime.Date.StartOfWeek(DayOfWeek.Monday))
                .OrderByDescending(h => h.Key)
                .Select(h =>
                (
                    day: h.Key,
                    cost: h.Sum(e => e.PriceConsumption),
                    charge: h.Sum(e => e.Consumption)
                ))
                .ToArray();
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
