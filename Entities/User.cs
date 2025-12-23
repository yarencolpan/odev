using System.Collections.Generic;

namespace odev.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // In real app, hash this
        public string Role { get; set; } = "User"; // User, Admin

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
