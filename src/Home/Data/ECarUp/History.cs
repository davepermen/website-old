using System;

namespace Home.Data.ECarUp
{
    public class History
    {
        public string StationName { get; set; } = "";

        public DateTime DateTime { get; set; } = DateTime.MinValue;

        public int Durration { get; set; } = 0;

        public double Consumption { get; set; } = 0;

        public double PriceConsumption { get; set; } = 0;

        public double PriceParkingTime { get; set; } = 0;

        public string? PriceUnit { get; set; } = "";
    }
}
