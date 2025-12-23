using System;
using System.Collections.Generic;

namespace odev.Entities
{
    public class Flight : BaseEntity
    {
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        
        // Soft delete bonus

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
