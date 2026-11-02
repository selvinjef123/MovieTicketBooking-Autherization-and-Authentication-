using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    public BookingsController(IBookingService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? showId, [FromQuery] BookingStatus? status) =>
        Ok(await _service.GetAllAsync(showId, status));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _service.GetByIdAsync(id);
        return booking == null ? NotFound() : Ok(booking);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        var (response, error, statusCode) = await _service.CreateBookingAsync(request);
        return statusCode switch
        {
            201 => CreatedAtAction(nameof(GetById), new { id = response!.BookingId }, response),
            400 => BadRequest(new { message = error }),
            404 => NotFound(new { message = error }),
            409 => Conflict(new { message = error }),
            _ => StatusCode(500, "An unexpected error occurred.")
        };
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var (success, error, statusCode) = await _service.CancelBookingAsync(id);
        return statusCode switch
        {
            200 => Ok(new { message = "Booking successfully cancelled." }),
            400 => BadRequest(new { message = error }),
            404 => NotFound(new { message = error }),
            _ => StatusCode(500, "An unexpected error occurred.")
        };
    }
}