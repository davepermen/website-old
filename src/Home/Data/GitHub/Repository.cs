using System;
using System.Text.Json.Serialization;

namespace Home.Data.GitHub
{
    public class Repository
    {
        public string Name { get; set; } = "";
        [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = "";
        [JsonPropertyName("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.MinValue;
    }
}
