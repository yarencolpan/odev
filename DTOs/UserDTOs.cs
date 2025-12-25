using System.ComponentModel.DataAnnotations;

namespace odev.DTOs
{
    public record CreateUserDto([Required] string Username, [Required] string Password, string Role = "User");
    public record UpdateUserDto(string Username, string Role);
    public record UserResponseDto(int Id, string Username, string Role, DateTime CreatedAt, DateTime? UpdatedAt);
    public record LoginDto([Required] string Username, [Required] string Password);
}
