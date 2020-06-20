using System;
using System.Text.Json.Serialization;

namespace Home.Data.Tesla
{
    public class Vehicle
    {

        [JsonPropertyName("id")] public long Id { get; set; } = 0;
        [JsonPropertyName("vehicle_id")] public int VehicleId { get; set; } = 0;
        [JsonPropertyName("vin")] public string Vin { get; set; } = "";
        [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
        [JsonPropertyName("option_codes")] public string OptionCodes { get; set; } = "";
        [JsonPropertyName("color")] public string Color { get; set; } = "";
        [JsonPropertyName("tokens")] public string[] Tokens { get; set; } = Array.Empty<string>();
        [JsonPropertyName("state")] public string State { get; set; } = "";
        [JsonPropertyName("in_service")] public bool InService { get; set; } = false;
        [JsonPropertyName("id_s")] public string IdS { get; set; } = "";
        [JsonPropertyName("calendar_enabled")] public bool CalendarEnabled { get; set; } = false;
        [JsonPropertyName("backseat_token")] public string BackseatToken { get; set; } = "";
        [JsonPropertyName("backseat_token_updated_at")] public string BackseatTokenUpdatedAt { get; set; } = "";
    }

}
