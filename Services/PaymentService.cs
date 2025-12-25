using Microsoft.EntityFrameworkCore;
using odev.Data;
using odev.DTOs;
using odev.Entities;

namespace odev.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse<List<PaymentResponseDto>>> GetAllAsync();
        Task<ApiResponse<PaymentResponseDto>> CreateAsync(CreatePaymentDto dto);
    }

    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<PaymentResponseDto>>> GetAllAsync()
        {
            var payments = await _context.Payments
                .Select(p => new PaymentResponseDto(p.Id, p.BookingId, p.Amount, p.PaymentMethod, p.PaymentDate, p.CreatedAt))
                .ToListAsync();
            return ApiResponse<List<PaymentResponseDto>>.Ok(payments);
        }

        public async Task<ApiResponse<PaymentResponseDto>> CreateAsync(CreatePaymentDto dto)
        {
            var booking = await _context.Bookings.FindAsync(dto.BookingId);
            if (booking == null) return ApiResponse<PaymentResponseDto>.Fail("Booking not found");

            var payment = new Payment
            {
                BookingId = dto.BookingId,
                Amount = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return ApiResponse<PaymentResponseDto>.Ok(new PaymentResponseDto(payment.Id, payment.BookingId, payment.Amount, payment.PaymentMethod, payment.PaymentDate, payment.CreatedAt), "Payment successful");
        }
    }
}
