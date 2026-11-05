using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Auth;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUserService _userService) : ControllerBase
{
	[HttpPost("register")]
	public async Task<IActionResult> Register(RegisterDto dto)
	{
		var result = await _userService.RegisterAsync(dto);

		if (!result.Success)
			return BadRequest(new { message = result.Message });

		return Ok(new { message = result.Message });
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login(LoginDto dto)
	{
		var result = await _userService.LoginAsync(dto);
		if (!result.Success)
		{
			return Unauthorized(new { message = result.Message });
		}

		return Ok(new
		{
			message = result.Message,
			userId = result.UserId,
			expiresAt = result.ExpiresAt,
			token = result.Token
		});
	}

	[Authorize]
	[HttpGet("me")]
	public IActionResult GetCurrentUser()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		var name = User.FindFirstValue(ClaimTypes.Name);
		var email = User.FindFirstValue(ClaimTypes.Email);
		var role = User.FindFirstValue(ClaimTypes.Role);

		return Ok(new
		{
			userId = int.TryParse(userId, out var id) ? id : 0,
			fullName = name,
			email = email,
			role = role
		});
	}
}
