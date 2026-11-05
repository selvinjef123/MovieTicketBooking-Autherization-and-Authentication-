using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/theatres")]
[ApiController]
public class TheatresController : ControllerBase
{
    private readonly ITheatreService _theatreService;
    public TheatresController(ITheatreService theatreService) => _theatreService = theatreService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TheatreResponse>>> GetAll() => Ok(await _theatreService.GetAllTheatresAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TheatreResponse>> GetById(int id)
    {
        var theatre = await _theatreService.GetTheatreByIdAsync(id);
        if (theatre == null) return NotFound();
        return Ok(theatre);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<TheatreResponse>> Create([FromBody] CreateTheatreRequest request)
    {
        var created = await _theatreService.CreateTheatreAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.TheatreId }, created);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTheatreRequest request)
    {
        if (!await _theatreService.UpdateTheatreAsync(id, request)) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _theatreService.DeleteTheatreAsync(id)) return Conflict("Cannot delete theatre due to existing dependencies.");
        return NoContent();
    }
}
