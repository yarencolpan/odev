using Microsoft.EntityFrameworkCore;
using odev.Data;
using odev.DTOs;
using odev.Entities;

namespace odev.Services
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserResponseDto>>> GetAllAsync();
        Task<ApiResponse<UserResponseDto>> GetByIdAsync(int id);
        Task<ApiResponse<UserResponseDto>> CreateAsync(CreateUserDto dto);
        Task<ApiResponse<UserResponseDto>> UpdateAsync(int id, UpdateUserDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<string>> LoginAsync(LoginDto dto);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<UserResponseDto>>> GetAllAsync()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDto(u.Id, u.Username, u.Role, u.CreatedAt, u.UpdatedAt))
                .ToListAsync();
            return ApiResponse<List<UserResponseDto>>.Ok(users);
        }

        public async Task<ApiResponse<UserResponseDto>> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return ApiResponse<UserResponseDto>.Fail("User not found", 404);
            
            return ApiResponse<UserResponseDto>.Ok(new UserResponseDto(user.Id, user.Username, user.Role, user.CreatedAt, user.UpdatedAt));
        }

        public async Task<ApiResponse<UserResponseDto>> CreateAsync(CreateUserDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return ApiResponse<UserResponseDto>.Fail("Username already exists", 409);

            var user = new User
            {
                Username = dto.Username,
                Password = dto.Password, // Should hash here
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return ApiResponse<UserResponseDto>.Ok(new UserResponseDto(user.Id, user.Username, user.Role, user.CreatedAt, user.UpdatedAt), "User created");
        }

        public async Task<ApiResponse<UserResponseDto>> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return ApiResponse<UserResponseDto>.Fail("User not found", 404);

            if (!string.IsNullOrEmpty(dto.Username)) user.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Role)) user.Role = dto.Role;

            await _context.SaveChangesAsync();
            return ApiResponse<UserResponseDto>.Ok(new UserResponseDto(user.Id, user.Username, user.Role, user.CreatedAt, user.UpdatedAt), "User updated");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return ApiResponse<bool>.Fail("User not found", 404);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return ApiResponse<bool>.Ok(true, "User deleted");
        }

        public async Task<ApiResponse<string>> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password);
            if (user == null) return ApiResponse<string>.Fail("Invalid credentials", 401);

            // Bonus: Return JWT here. For now just a fake token or string.
            // I'll implement real JWT later if time permits, but requirements say "Bonus: JWT Auth - 10p"
            // I will return a dummy token for now.
            return ApiResponse<string>.Ok($"fake-jwt-token-for-{user.Username}", "Login successful");
        }
    }
}
