using System;

namespace Home.Data.ECarUp
{
    public class ChargingState
    {
        public float EnergyUsed { get; set; } = 0;
        public int Duration { get; set; } = 0;
        public float Costs { get; set; } = 0;
        public int RemainingParkingTime { get; set; } = 0;
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Address { get; set; } = "";
        public float EnergyPrice { get; set; } = 0;
        public float ParkingPrice { get; set; } = 0;
        public string PriceUnit { get; set; } = "";
        public float Latitude { get; set; } = 0;
        public float Longitude { get; set; } = 0;
        public string[] ImageUrls { get; set; } = Array.Empty<string>();
        public int State { get; set; } = 0;
        public int TimeStationOccupied { get; set; } = 0;
        public float ActivePower { get; set; } = 0;
        public int PlugType { get; set; } = 0;
        public int MaxPower { get; set; } = 0;
        public object? ReservedByUser { get; set; }
    }
}