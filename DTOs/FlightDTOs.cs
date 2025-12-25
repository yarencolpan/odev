using System.ComponentModel.DataAnnotations;

namespace odev.DTOs
{
    public record CreateFlightDto(
        [Required] string FlightNumber, 
        [Required] string Origin, 
        [Required] string Destination, 
        DateTime DepartureTime, 
        DateTime ArrivalTime, 
        decimal Price, 
        int Capacity
    );

    public record UpdateFlightDto(
        string FlightNumber, 
        string Origin, 
        string Destination, 
        DateTime? DepartureTime, 
        DateTime? ArrivalTime, 
        decimal? Price, 
        int? Capacity
    );

    public record FlightResponseDto(
        int Id, 
        string FlightNumber, 
        string Origin, 
        string Destination, 
        DateTime DepartureTime, 
        DateTime ArrivalTime, 
        decimal Price, 
        int Capacity,
        bool IsDeleted,
        DateTime CreatedAt
    );
}
