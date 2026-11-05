using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<IEnumerable<MovieResponse>> GetAllMoviesAsync()
    {
        var movies = await _movieRepository.GetAllAsync();
        return movies.Select(m => new MovieResponse
        {
            MovieId = m.MovieId,
            Title = m.Title,
            Description = m.Description,
            DurationMinutes = m.DurationMinutes,
            ReleaseDate = m.ReleaseDate,
            Language = m.Language,
            Genre = m.Genre
        });
    }

    public async Task<MovieResponse?> GetMovieByIdAsync(int id)
    {
        var m = await _movieRepository.GetByIdAsync(id);
        if (m == null) return null;

        return new MovieResponse
        {
            MovieId = m.MovieId,
            Title = m.Title,
            Description = m.Description,
            DurationMinutes = m.DurationMinutes,
            ReleaseDate = m.ReleaseDate,
            Language = m.Language,
            Genre = m.Genre
        };
    }

    public async Task<IEnumerable<MovieResponse>> SearchMoviesAsync(string title)
    {
        var movies = await _movieRepository.SearchByTitleAsync(title);
        return movies.Select(m => new MovieResponse
        {
            MovieId = m.MovieId,
            Title = m.Title,
            Description = m.Description,
            DurationMinutes = m.DurationMinutes,
            ReleaseDate = m.ReleaseDate,
            Language = m.Language,
            Genre = m.Genre
        });
    }

    public async Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            ReleaseDate = request.ReleaseDate,
            Language = request.Language,
            Genre = request.Genre
        };

        var created = await _movieRepository.AddAsync(movie);

        return new MovieResponse
        {
            MovieId = created.MovieId,
            Title = created.Title,
            Description = created.Description,
            DurationMinutes = created.DurationMinutes,
            ReleaseDate = created.ReleaseDate,
            Language = created.Language,
            Genre = created.Genre
        };
    }

    public async Task<bool> UpdateMovieAsync(int id, UpdateMovieRequest request)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null) return false;

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.ReleaseDate = request.ReleaseDate;
        movie.Language = request.Language;
        movie.Genre = request.Genre;

        await _movieRepository.UpdateAsync(movie);
        return true;
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null) return false;

        try
        {
            await _movieRepository.DeleteAsync(movie);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
