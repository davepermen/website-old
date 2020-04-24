using System.Text.Json.Serialization;

namespace Home.Data.ExchangeRates
{
    public class Response
    {
        [JsonPropertyName("rates")] public Rates Rates { get; set; } = new Rates();
    }
}
