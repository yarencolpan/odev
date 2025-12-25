using System.ComponentModel.DataAnnotations;

namespace odev.DTOs
{
    public record CreatePaymentDto(
        [Required] int BookingId,
        [Required] decimal Amount,
        string PaymentMethod = "CreditCard"
    );

    public record PaymentResponseDto(
        int Id,
        int BookingId,
        decimal Amount,
        string PaymentMethod,
        DateTime PaymentDate,
        DateTime CreatedAt
    );
}
