using System.Text.Json.Serialization;

namespace Home.Data.FoldingAtHome.Client
{
    public class Slot
    {
        [JsonPropertyName("status")] public string Status { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
        [JsonPropertyName("percentdone")] public string PercentDone { get; set; } = "";
        [JsonPropertyName("eta")] public string Eta { get; set; } = "";
    }
}
