using System.Text.Json.Serialization;

namespace Home.Data.AlphaVantage
{
    public class GlobalQuote
    {
        [JsonPropertyName("05. price")] public string Price { get; set; } = "";
    }
}
