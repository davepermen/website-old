namespace EvHomeCharging
{
    public class ChargingStation
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public float EnergyPrice { get; set; }
        public float ParkingPrice { get; set; }
        public string PriceUnit { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string[] ImageUrls { get; set; }
        public int State { get; set; }
        public int TimeStationOccupied { get; set; }
        public float ActivePower { get; set; }
        public int PlugType { get; set; }
        public int MaxPower { get; set; }
        public object ReservedByUser { get; set; }
        public Reservationsettings ReservationSettings { get; set; }

        public class Reservationsettings
        {
            public bool IsEnabled { get; set; }
            public float InitialCost { get; set; }
            public float InitialCostGiveBack { get; set; }
            public int TimeSlotDurration { get; set; }
            public float CostOfSingleTimeSlot { get; set; }
        }
    }
}
