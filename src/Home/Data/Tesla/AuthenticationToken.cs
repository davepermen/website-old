using System.Text.Json.Serialization;

namespace Home.Data.Tesla
{
    public class AuthenticationToken
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = "";
        [JsonPropertyName("token_type")] public string TokenType { get; set; } = "";
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; } = 0;
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = "";
        [JsonPropertyName("created_at")] public int CreatedAt { get; set; } = 0;
    }

}
