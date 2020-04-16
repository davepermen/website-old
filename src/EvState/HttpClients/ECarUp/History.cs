using System;

namespace EvState.HttpClients.ECarUp
{
    public class History
    {
        public string StationName { get; set; }

        public DateTime DateTime { get; set; }

        public int Durration { get; set; }

        public double Consumption { get; set; }

        public double PriceConsumption { get; set; }

        public double PriceParkingTime { get; set; }

        public string PriceUnit { get; set; }
    }
}
