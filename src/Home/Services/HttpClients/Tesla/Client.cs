using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Home.Services.HttpClients.Tesla
{
    public class Client
    {
        private readonly HttpClient client;
        private readonly IConfigurationSection configuration;

        DateTime lastLogin = DateTime.MinValue;

        public Client(HttpClient client, IConfiguration configuration)
        {
            this.configuration = configuration.GetSection("tesla");

            this.client = client;

            client.BaseAddress = new Uri(@"https://owner-api.teslamotors.com");
            client.DefaultRequestHeaders.Add("X-Tesla-User-Agent", "TeslaApp/3.4.4-350/fad4a582e/android/9.0.0");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 9.0.0; VS985 4G Build/LRX21Y; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/58.0.3029.83 Mobile Safari/537.36");
        }

        async Task Login()
        {
            if (client.DefaultRequestHeaders.Contains("authorization") == false || DateTime.UtcNow - lastLogin > TimeSpan.FromDays(1))
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "client_id", configuration["clientid"] },
                    { "client_secret", configuration["secret"] },
                    { "email", configuration["email"] },
                    { "password", configuration["password"] },
                });
                var authenticationToken = await client.PostAsync<Tesla.AuthenticationToken>("/oauth/token", content);
                if (client.DefaultRequestHeaders.Contains("authorization"))
                {
                    client.DefaultRequestHeaders.Remove("authorization");
                }
                client.DefaultRequestHeaders.Add("authorization", $"Bearer {authenticationToken.access_token}");
                lastLogin = DateTime.UtcNow;
            }
        }

        public async Task WakeUpCars()
        {
            await Login();

            var vehicles = await client.GetAsync<Tesla.Vehicles>("/api/1/vehicles");

            foreach (var vehicle in vehicles.response)
            {
                await client.PostAsync($"/api/1/vehicles/{vehicle.id}/wake_up", null);
            }
        }
    }
    public static class PostHelper
    {
        public static async Task<T> PostAsync<T>(this HttpClient httpClient, string requestUri, HttpContent content)
        {
            var result = await httpClient.PostAsync(requestUri, content);
            var _content = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(_content);
        }
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri)
        {
            var result = await httpClient.GetAsync(requestUri);
            var _content = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(_content);
        }
    }

    namespace Tesla
    {
        public class AuthenticationToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
            public int created_at { get; set; }
        }

        public class Response<T>
        {
            public T response { get; set; }
        }

        public class Vehicles
        {
            public Vehicle[] response { get; set; }
            public int count { get; set; }
        }

        public class Vehicle
        {
            public long id { get; set; }
            public int vehicle_id { get; set; }
            public string vin { get; set; }
            public string display_name { get; set; }
            public string option_codes { get; set; }
            public object color { get; set; }
            public string[] tokens { get; set; }
            public string state { get; set; }
            public bool in_service { get; set; }
            public string id_s { get; set; }
            public bool calendar_enabled { get; set; }
            public int api_version { get; set; }
            public object backseat_token { get; set; }
            public object backseat_token_updated_at { get; set; }
        }
        public class State
        {
            public long id { get; set; }
            public int user_id { get; set; }
            public int vehicle_id { get; set; }
            public string vin { get; set; }
            public string display_name { get; set; }
            public string option_codes { get; set; }
            public object color { get; set; }
            public string[] tokens { get; set; }
            public string state { get; set; }
            public bool in_service { get; set; }
            public string id_s { get; set; }
            public bool calendar_enabled { get; set; }
            public int api_version { get; set; }
            public object backseat_token { get; set; }
            public object backseat_token_updated_at { get; set; }
            public Charge_State charge_state { get; set; }
            public Climate_State climate_state { get; set; }
            public Drive_State drive_state { get; set; }
            public Gui_Settings gui_settings { get; set; }
            public Vehicle_Config vehicle_config { get; set; }
            public Vehicle_State vehicle_state { get; set; }
        }

        public class Charge_State
        {
            public bool battery_heater_on { get; set; }
            public int battery_level { get; set; }
            public float battery_range { get; set; }
            public int charge_current_request { get; set; }
            public int charge_current_request_max { get; set; }
            public bool charge_enable_request { get; set; }
            public float charge_energy_added { get; set; }
            public int charge_limit_soc { get; set; }
            public int charge_limit_soc_max { get; set; }
            public int charge_limit_soc_min { get; set; }
            public int charge_limit_soc_std { get; set; }
            public float charge_miles_added_ideal { get; set; }
            public float charge_miles_added_rated { get; set; }
            public bool charge_port_cold_weather_mode { get; set; }
            public bool charge_port_door_open { get; set; }
            public string charge_port_latch { get; set; }
            public float charge_rate { get; set; }
            public bool charge_to_max_range { get; set; }
            public int charger_actual_current { get; set; }
            public object charger_phases { get; set; }
            public int charger_pilot_current { get; set; }
            public int charger_power { get; set; }
            public int charger_voltage { get; set; }
            public string charging_state { get; set; }
            public string conn_charge_cable { get; set; }
            public float est_battery_range { get; set; }
            public string fast_charger_brand { get; set; }
            public bool fast_charger_present { get; set; }
            public string fast_charger_type { get; set; }
            public float ideal_battery_range { get; set; }
            public bool managed_charging_active { get; set; }
            public object managed_charging_start_time { get; set; }
            public bool managed_charging_user_canceled { get; set; }
            public int max_range_charge_counter { get; set; }
            public int minutes_to_full_charge { get; set; }
            public object not_enough_power_to_heat { get; set; }
            public bool scheduled_charging_pending { get; set; }
            public object scheduled_charging_start_time { get; set; }
            public float time_to_full_charge { get; set; }
            public long timestamp { get; set; }
            public bool trip_charging { get; set; }
            public int usable_battery_level { get; set; }
            public object user_charge_enable_request { get; set; }
        }

        public class Climate_State
        {
            public bool battery_heater { get; set; }
            public object battery_heater_no_power { get; set; }
            public string climate_keeper_mode { get; set; }
            public float driver_temp_setting { get; set; }
            public int fan_status { get; set; }
            public float inside_temp { get; set; }
            public bool is_auto_conditioning_on { get; set; }
            public bool is_climate_on { get; set; }
            public bool is_front_defroster_on { get; set; }
            public bool is_preconditioning { get; set; }
            public bool is_rear_defroster_on { get; set; }
            public int left_temp_direction { get; set; }
            public float max_avail_temp { get; set; }
            public float min_avail_temp { get; set; }
            public float outside_temp { get; set; }
            public float passenger_temp_setting { get; set; }
            public bool remote_heater_control_enabled { get; set; }
            public int right_temp_direction { get; set; }
            public int seat_heater_left { get; set; }
            public int seat_heater_right { get; set; }
            public bool side_mirror_heaters { get; set; }
            public bool smart_preconditioning { get; set; }
            public long timestamp { get; set; }
            public bool wiper_blade_heater { get; set; }
        }

        public class Drive_State
        {
            public int gps_as_of { get; set; }
            public int heading { get; set; }
            public float latitude { get; set; }
            public float longitude { get; set; }
            public float native_latitude { get; set; }
            public int native_location_supported { get; set; }
            public float native_longitude { get; set; }
            public string native_type { get; set; }
            public int power { get; set; }
            public object shift_state { get; set; }
            public object speed { get; set; }
            public long timestamp { get; set; }
        }

        public class Gui_Settings
        {
            public bool gui_24_hour_time { get; set; }
            public string gui_charge_rate_units { get; set; }
            public string gui_distance_units { get; set; }
            public string gui_range_display { get; set; }
            public string gui_temperature_units { get; set; }
            public bool show_range_units { get; set; }
            public long timestamp { get; set; }
        }

        public class Vehicle_Config
        {
            public bool can_accept_navigation_requests { get; set; }
            public bool can_actuate_trunks { get; set; }
            public string car_special_type { get; set; }
            public string car_type { get; set; }
            public string charge_port_type { get; set; }
            public bool eu_vehicle { get; set; }
            public string exterior_color { get; set; }
            public bool has_air_suspension { get; set; }
            public bool has_ludicrous_mode { get; set; }
            public int key_version { get; set; }
            public bool motorized_charge_port { get; set; }
            public bool plg { get; set; }
            public int rear_seat_heaters { get; set; }
            public object rear_seat_type { get; set; }
            public bool rhd { get; set; }
            public string roof_color { get; set; }
            public object seat_type { get; set; }
            public string spoiler_type { get; set; }
            public object sun_roof_installed { get; set; }
            public string third_row_seats { get; set; }
            public long timestamp { get; set; }
            public string trim_badging { get; set; }
            public bool use_range_badging { get; set; }
            public string wheel_type { get; set; }
        }

        public class Vehicle_State
        {
            public int api_version { get; set; }
            public string autopark_state_v2 { get; set; }
            public string autopark_style { get; set; }
            public bool calendar_supported { get; set; }
            public string car_version { get; set; }
            public int center_display_state { get; set; }
            public int df { get; set; }
            public int dr { get; set; }
            public int ft { get; set; }
            public bool homelink_nearby { get; set; }
            public bool is_user_present { get; set; }
            public string last_autopark_error { get; set; }
            public bool locked { get; set; }
            public Media_State media_state { get; set; }
            public bool notifications_supported { get; set; }
            public float odometer { get; set; }
            public bool parsed_calendar_supported { get; set; }
            public int pf { get; set; }
            public int pr { get; set; }
            public bool remote_start { get; set; }
            public bool remote_start_enabled { get; set; }
            public bool remote_start_supported { get; set; }
            public int rt { get; set; }
            public bool sentry_mode { get; set; }
            public bool sentry_mode_available { get; set; }
            public Software_Update software_update { get; set; }
            public Speed_Limit_Mode speed_limit_mode { get; set; }
            public object sun_roof_percent_open { get; set; }
            public string sun_roof_state { get; set; }
            public long timestamp { get; set; }
            public bool valet_mode { get; set; }
            public bool valet_pin_needed { get; set; }
            public string vehicle_name { get; set; }
        }

        public class Media_State
        {
            public bool remote_control_enabled { get; set; }
        }

        public class Software_Update
        {
            public int expected_duration_sec { get; set; }
            public string status { get; set; }
        }

        public class Speed_Limit_Mode
        {
            public bool active { get; set; }
            public float current_limit_mph { get; set; }
            public int max_limit_mph { get; set; }
            public int min_limit_mph { get; set; }
            public bool pin_code_set { get; set; }
        }

    }
}
