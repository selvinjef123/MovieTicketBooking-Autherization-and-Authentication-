using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers;

[Route("api/movies")]
[ApiController]
public class MoviesController : ControllerBase
{
	private readonly IMovieService _movieService;

	public MoviesController(IMovieService movieService)
	{
		_movieService = movieService;
	}

	[HttpGet]
	public async Task<ActionResult<IEnumerable<MovieResponse>>> GetAllMovies()
	{
		var movies = await _movieService.GetAllMoviesAsync();
		return Ok(movies);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<MovieResponse>> GetMovieById(int id)
	{
		var movie = await _movieService.GetMovieByIdAsync(id);
		if (movie == null) return NotFound();
		return Ok(movie);
	}

	[HttpGet("search")]
	public async Task<ActionResult<IEnumerable<MovieResponse>>> SearchMovies([FromQuery] string title)
	{
		if (string.IsNullOrWhiteSpace(title)) return BadRequest("Title query parameter is required.");
		var movies = await _movieService.SearchMoviesAsync(title);
		return Ok(movies);
	}

	[Authorize(Roles = UserRoles.Admin)]
	[HttpPost]
	public async Task<ActionResult<MovieResponse>> CreateMovie([FromBody] CreateMovieRequest request)
	{
		var created = await _movieService.CreateMovieAsync(request);
		return CreatedAtAction(nameof(GetMovieById), new { id = created.MovieId }, created);
	}

	[Authorize(Roles = UserRoles.Admin)]
	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateMovie(int id, [FromBody] UpdateMovieRequest request)
	{
		var success = await _movieService.UpdateMovieAsync(id, request);
		if (!success) return NotFound();
		return NoContent();
	}

	[Authorize(Roles = UserRoles.Admin)]
	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteMovie(int id)
	{
		var success = await _movieService.DeleteMovieAsync(id);
		if (!success) return Conflict("Cannot delete movie due to existing shows or dependencies.");
		return NoContent();
	}
}
