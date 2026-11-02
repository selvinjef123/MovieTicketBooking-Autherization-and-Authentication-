using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Auth;

public record RegisterRequest(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress, StringLength(150)] string Email,
    [Required, MinLength(6)] string Password,
    [Required, Phone, StringLength(15)] string Phone,
    string Role = "Customer"
);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password
);

public record AuthResponse(
    int UserId,
    string Name,
    string Email,
    string Role,
    string Token
);