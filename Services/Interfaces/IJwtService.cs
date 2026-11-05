using MovieTicketBooking.Models;

namespace MovieTicketBooking.Services.Interfaces;

public interface IJwtService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}

