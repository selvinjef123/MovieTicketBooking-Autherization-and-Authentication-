using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/seats")]
[ApiController]
public class SeatsController : ControllerBase
{
    private readonly ISeatService _seatService;
    public SeatsController(ISeatService seatService) => _seatService = seatService;

    [HttpGet("screen/{screenId:int}")]
    public async Task<ActionResult<IEnumerable<SeatResponse>>> GetByScreen(int screenId) => Ok(await _seatService.GetSeatsByScreenIdAsync(screenId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SeatResponse>> GetById(int id)
    {
        var seat = await _seatService.GetSeatByIdAsync(id);
        if (seat == null) return NotFound();
        return Ok(seat);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<SeatResponse>> Create([FromBody] CreateSeatRequest request)
    {
        var created = await _seatService.CreateSeatAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.SeatId }, created);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost("bulk")]
    public async Task<ActionResult<IEnumerable<SeatResponse>>> BulkCreate([FromBody] BulkCreateSeatsRequest request)
    {
        var createdSeats = await _seatService.BulkCreateSeatsAsync(request);
        return Ok(createdSeats);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSeatRequest request)
    {
        if (!await _seatService.UpdateSeatAsync(id, request)) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _seatService.DeleteSeatAsync(id)) return Conflict("Cannot delete seat due to existing bookings.");
        return NoContent();
    }
}
