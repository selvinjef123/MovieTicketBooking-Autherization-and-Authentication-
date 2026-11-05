using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/shows")]
[ApiController]
public class ShowsController : ControllerBase
{
    private readonly IShowService _showService;
    public ShowsController(IShowService showService) => _showService = showService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShowResponse>>> GetAll() => Ok(await _showService.GetAllShowsAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShowResponse>> GetById(int id)
    {
        var show = await _showService.GetShowByIdAsync(id);
        if (show == null) return NotFound();
        return Ok(show);
    }

    [HttpGet("movie/{movieId:int}")]
    public async Task<ActionResult<IEnumerable<ShowResponse>>> GetByMovie(int movieId) => Ok(await _showService.GetShowsByMovieIdAsync(movieId));

    [HttpGet("theatre/{theatreId:int}")]
    public async Task<ActionResult<IEnumerable<ShowResponse>>> GetByTheatre(int theatreId) => Ok(await _showService.GetShowsByTheatreIdAsync(theatreId));

    [HttpGet("{showId:int}/seats")]
    public async Task<ActionResult<IEnumerable<ShowSeatResponse>>> GetShowSeats(int showId)
    {
        var show = await _showService.GetShowByIdAsync(showId);
        if (show == null) return NotFound();
        return Ok(await _showService.GetShowSeatsAsync(showId));
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ShowResponse>> Create([FromBody] CreateShowRequest request)
    {
        try
        {
            var created = await _showService.CreateShowAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.ShowId }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateShowRequest request)
    {
        try
        {
            if (!await _showService.UpdateShowAsync(id, request)) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await _showService.DeleteShowAsync(id)) return Conflict("Cannot delete show due to existing bookings.");
        return NoContent();
    }
}
