using System.ComponentModel.DataAnnotations;

namespace odev.DTOs
{
    public record CreateBookingDto(
        [Required] int UserId,
        [Required] int FlightId,
        [Required] string SeatNumber
    );

    public record UpdateBookingDto(string Status, string SeatNumber);

    public record BookingResponseDto(
        int Id,
        int UserId,
        string Username,
        int FlightId,
        string FlightNumber,
        string SeatNumber,
        string Status,
        DateTime CreatedAt
    );
}
