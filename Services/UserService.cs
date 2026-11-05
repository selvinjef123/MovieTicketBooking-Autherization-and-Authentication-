using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class UserService(
	IUserRepository repo,
	IPasswordHasher<User> passwordHasher,
	IJwtService jwtService) : IUserService
{
	public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto dto)
	{
		var email = dto.Email.Trim().ToLower();
		var existingUser = await repo.GetByEmailAsync(email);
		if (existingUser != null)
		{
			return (false, "Email is already registered.");
		}

		var user = new User
		{
			FullName = dto.FullName,
			Email = email,
			Role = UserRoles.Customer
		};

		user.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
		await repo.AddAsync(user);

		return (true, "Registration successful.");
	}

	public async Task<(bool Success, string Message, string? Token, int? UserId, DateTime? ExpiresAt)> LoginAsync(LoginDto dto)
	{
		var email = dto.Email.Trim().ToLower();
		var user = await repo.GetByEmailAsync(email);

		if (user is null)
		{
			return (false, "Invalid email or password.", null, null, null);
		}

		var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
		if (result == PasswordVerificationResult.Failed)
		{
			return (false, "Invalid email or password.", null, null, null);
		}

		var (token, expiresAt) = jwtService.GenerateToken(user);
		return (true, "Login successful.", token, user.UserId, expiresAt);
	}

	public async Task CreateRoleUserAsync(
		string fullName,
		string email,
		string password,
		string role)
	{
		var normalizedEmail = email.Trim().ToLower();
		var existingUser = await repo.GetByEmailAsync(normalizedEmail);
		if (existingUser != null)
		{
			return;
		}

		var user = new User
		{
			FullName = fullName,
			Email = normalizedEmail,
			Role = role
		};

		user.PasswordHash = passwordHasher.HashPassword(user, password);
		await repo.AddAsync(user);
	}
}

