using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _service;
    public MoviesController(IMovieService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? title = null) =>
        Ok(await _service.GetAllAsync(pageNumber, pageSize, title));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var movie = await _service.GetByIdAsync(id);
        return movie == null ? NotFound() : Ok(movie);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string title) =>
        Ok(await _service.GetAllAsync(1, 20, title));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateMovieRequest request)
    {
        var created = await _service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.MovieId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMovieRequest request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return updated ? Ok() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, errorMessage) = await _service.DeleteAsync(id);
        if (!success && errorMessage == null) return NotFound();
        if (!success) return Conflict(new { message = errorMessage });
        return NoContent();
    }
}