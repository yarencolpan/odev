using System.Collections.Generic;

namespace odev.Entities
{
    public class Booking : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int? FlightId { get; set; }
        public Flight? Flight { get; set; }

        public string SeatNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
