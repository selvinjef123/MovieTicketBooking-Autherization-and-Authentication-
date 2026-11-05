using MovieTicketBooking.DTOs.Movies;

namespace MovieTicketBooking.Services.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieResponse>> GetAllMoviesAsync();
    Task<MovieResponse?> GetMovieByIdAsync(int id);
    Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string title);
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);
    Task<bool> UpdateMovieAsync(int id, UpdateMovieRequest request);
    Task<bool> DeleteMovieAsync(int id);
}
