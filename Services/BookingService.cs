using Microsoft.EntityFrameworkCore;
using odev.Data;
using odev.DTOs;
using odev.Entities;

namespace odev.Services
{
    public interface IBookingService
    {
        Task<ApiResponse<List<BookingResponseDto>>> GetAllAsync();
        Task<ApiResponse<BookingResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<BookingResponseDto>> CreateAsync(CreateBookingDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<BookingResponseDto>>> GetAllAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Flight)
                .Select(b => new BookingResponseDto(b.Id, b.UserId, b.User.Username, b.FlightId.GetValueOrDefault(), b.Flight == null ? "Unknown" : b.Flight.FlightNumber, b.SeatNumber, b.Status, b.CreatedAt))
                .ToListAsync();
            return ApiResponse<List<BookingResponseDto>>.Ok(bookings);
        }

        public async Task<ApiResponse<BookingResponseDto>> GetByIdAsync(int id)
        {
            var b = await _context.Bookings
                .Include(x => x.User)
                .Include(x => x.Flight)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (b == null) return ApiResponse<BookingResponseDto>.Fail("Booking not found");

            return ApiResponse<BookingResponseDto>.Ok(new BookingResponseDto(b.Id, b.UserId, b.User.Username, b.FlightId.GetValueOrDefault(), b.Flight?.FlightNumber ?? "Unknown", b.SeatNumber, b.Status, b.CreatedAt));
        }

        public async Task<ApiResponse<BookingResponseDto>> CreateAsync(CreateBookingDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null) return ApiResponse<BookingResponseDto>.Fail("User not found");

            var flight = await _context.Flights.FindAsync(dto.FlightId);
            if (flight == null) return ApiResponse<BookingResponseDto>.Fail("Flight not found");

            // Check capacity (simple check)
            var currentBookings = await _context.Bookings.CountAsync(b => b.FlightId == dto.FlightId && b.Status != "Cancelled");
            if (currentBookings >= flight.Capacity)
                return ApiResponse<BookingResponseDto>.Fail("Flight is full");

            var booking = new Booking
            {
                UserId = dto.UserId,
                FlightId = dto.FlightId,
                SeatNumber = dto.SeatNumber,
                Status = "Confirmed"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Re-fetch to get included data or just construct DTO manually
            return ApiResponse<BookingResponseDto>.Ok(new BookingResponseDto(
                booking.Id, 
                user.Id, 
                user.Username, 
                flight.Id, 
                flight.FlightNumber, 
                booking.SeatNumber, 
                booking.Status, 
                booking.CreatedAt), "Booking created");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return ApiResponse<bool>.Fail("Booking not found");

            booking.Status = "Cancelled";
            // _context.Bookings.Remove(booking); // Or just cancel? Requirement "CRUD" usually expects delete. I'll delete it.
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Booking deleted");
        }
    }
}
