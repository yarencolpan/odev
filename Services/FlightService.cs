using Microsoft.EntityFrameworkCore;
using odev.Data;
using odev.DTOs;
using odev.Entities;

namespace odev.Services
{
    public interface IFlightService
    {
        Task<ApiResponse<List<FlightResponseDto>>> GetAllAsync();
        Task<ApiResponse<FlightResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<FlightResponseDto>> CreateAsync(CreateFlightDto dto);
        Task<ApiResponse<FlightResponseDto>> UpdateAsync(int id, UpdateFlightDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }

    public class FlightService : IFlightService
    {
        private readonly AppDbContext _context;

        public FlightService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<FlightResponseDto>>> GetAllAsync()
        {
            var flights = await _context.Flights
                .Select(f => new FlightResponseDto(f.Id, f.FlightNumber, f.Origin, f.Destination, f.DepartureTime, f.ArrivalTime, f.Price, f.Capacity, false, f.CreatedAt))
                .ToListAsync();
            return ApiResponse<List<FlightResponseDto>>.Ok(flights);
        }

        public async Task<ApiResponse<FlightResponseDto>> GetByIdAsync(int id)
        {
            var f = await _context.Flights.FindAsync(id);
            if (f == null) return ApiResponse<FlightResponseDto>.Fail("Flight not found");

            return ApiResponse<FlightResponseDto>.Ok(new FlightResponseDto(f.Id, f.FlightNumber, f.Origin, f.Destination, f.DepartureTime, f.ArrivalTime, f.Price, f.Capacity, false, f.CreatedAt));
        }

        public async Task<ApiResponse<FlightResponseDto>> CreateAsync(CreateFlightDto dto)
        {
            var flight = new Flight
            {
                FlightNumber = dto.FlightNumber,
                Origin = dto.Origin,
                Destination = dto.Destination,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                Price = dto.Price,
                Capacity = dto.Capacity
            };

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return ApiResponse<FlightResponseDto>.Ok(new FlightResponseDto(flight.Id, flight.FlightNumber, flight.Origin, flight.Destination, flight.DepartureTime, flight.ArrivalTime, flight.Price, flight.Capacity, false, flight.CreatedAt), "Flight created");
        }

        public async Task<ApiResponse<FlightResponseDto>> UpdateAsync(int id, UpdateFlightDto dto)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return ApiResponse<FlightResponseDto>.Fail("Flight not found");

            if (!string.IsNullOrEmpty(dto.FlightNumber)) flight.FlightNumber = dto.FlightNumber;
            if (!string.IsNullOrEmpty(dto.Origin)) flight.Origin = dto.Origin;
            if (!string.IsNullOrEmpty(dto.Destination)) flight.Destination = dto.Destination;
            if (dto.DepartureTime.HasValue) flight.DepartureTime = dto.DepartureTime.Value;
            if (dto.ArrivalTime.HasValue) flight.ArrivalTime = dto.ArrivalTime.Value;
            if (dto.Price.HasValue) flight.Price = dto.Price.Value;
            if (dto.Capacity.HasValue) flight.Capacity = dto.Capacity.Value;

            await _context.SaveChangesAsync();

            return ApiResponse<FlightResponseDto>.Ok(new FlightResponseDto(flight.Id, flight.FlightNumber, flight.Origin, flight.Destination, flight.DepartureTime, flight.ArrivalTime, flight.Price, flight.Capacity, false, flight.CreatedAt), "Flight updated");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null) return ApiResponse<bool>.Fail("Flight not found");

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Flight deleted");
        }
    }
}
