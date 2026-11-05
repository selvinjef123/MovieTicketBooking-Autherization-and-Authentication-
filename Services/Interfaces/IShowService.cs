using MovieTicketBooking.DTOs.Shows;

namespace MovieTicketBooking.Services.Interfaces;

public interface IShowService
{
    Task<IEnumerable<ShowResponse>> GetAllShowsAsync();
    Task<ShowResponse?> GetShowByIdAsync(int id);
    Task<IEnumerable<ShowResponse>> GetShowsByMovieIdAsync(int movieId);
    Task<IEnumerable<ShowResponse>> GetShowsByTheatreIdAsync(int theatreId);
    Task<IEnumerable<ShowSeatResponse>> GetShowSeatsAsync(int showId);
    Task<ShowResponse> CreateShowAsync(CreateShowRequest request);
    Task<bool> UpdateShowAsync(int id, UpdateShowRequest request);
    Task<bool> DeleteShowAsync(int id);
}
