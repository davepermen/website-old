using System.Text.Json.Serialization;

namespace Home.Data.AlphaVantage
{
    public class Response
    {
        [JsonPropertyName("Global Quote")]
        public GlobalQuote GlobalQuote { get; set; } = new GlobalQuote();
    }
}
