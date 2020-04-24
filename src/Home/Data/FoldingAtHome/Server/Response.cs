using System;
using System.Text.Json.Serialization;

namespace Home.Data.FoldingAtHome.Server
{
    public class Response
    {
        public Team[] Teams { get; set; } = Array.Empty<Team>();
    }

    public class Team
    {
        [JsonPropertyName("wus")] public int WorkUnits { get; set; } = 0;
        public int Credit { get; set; } = 0;
        public string Name { get; set; } = "";
    }
}
