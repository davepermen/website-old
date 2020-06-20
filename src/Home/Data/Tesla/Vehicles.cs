using System;
using System.Text.Json.Serialization;

namespace Home.Data.Tesla
{
    public class Vehicles
    {
        [JsonPropertyName("response")] public Vehicle[] Response { get; set; } = Array.Empty<Vehicle>();
        [JsonPropertyName("count")] public int Count { get; set; }
    }

}
