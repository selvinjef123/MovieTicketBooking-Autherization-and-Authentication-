using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces;

public interface IUserService
{
	Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto);
	Task<(bool Success, string Message, string? Token, int? UserId, DateTime? ExpiresAt)> LoginAsync(LoginDto dto);
	Task CreateRoleUserAsync(string fullName, string email, string password, string role);
}

