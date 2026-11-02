using MovieTicketBooking.DTOs.Auth;

namespace MovieTicketBooking.Services.Interfaces;

public interface IAuthService
{
    Task<(AuthResponse? Response, string? ErrorMessage)> RegisterAsync(RegisterRequest request);
    Task<(AuthResponse? Response, string? ErrorMessage)> LoginAsync(LoginRequest request);
}