using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/screens")]
[ApiController]
public class ScreensController : ControllerBase
{
    private readonly IScreenService _screenService;
    public ScreensController(IScreenService screenService) => _screenService = screenService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScreenResponse>>> GetAll() => Ok(await _screenService.GetAllScreensAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ScreenResponse>> GetById(int id)
    {
        var screen = await _screenService.GetScreenByIdAsync(id);
        if (screen == null) return NotFound();
        return Ok(screen);
    }

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<ActionResult<IEnumerable<ScreenResponse>>> GetByTheatre(int theatreId) => Ok(await _screenService.GetScreensByTheatreIdAsync(theatreId));

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ScreenResponse>> Create([FromBody] CreateScreenRequest request)
    {
        var created = await _screenService.CreateScreenAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.ScreenId }, created);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateScreenRequest request)
    {
        if (!await _screenService.UpdateScreenAsync(id, request)) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _screenService.DeleteScreenAsync(id)) return Conflict("Cannot delete screen due to existing shows or seats.");
        return NoContent();
    }
}
