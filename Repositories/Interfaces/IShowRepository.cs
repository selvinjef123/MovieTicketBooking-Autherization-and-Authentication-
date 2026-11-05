using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IShowRepository
{
    Task<IEnumerable<Show>> GetAllAsync();
    Task<Show?> GetByIdAsync(int id);
    Task<IEnumerable<Show>> GetByMovieIdAsync(int movieId);
    Task<IEnumerable<Show>> GetByTheatreIdAsync(int theatreId);
    Task<IEnumerable<ShowSeat>> GetShowSeatsAsync(int showId);
    Task<ShowSeat?> GetShowSeatAsync(int showId, int seatId);
    Task<Show> AddAsync(Show show, IEnumerable<ShowSeat> showSeats);
    Task UpdateAsync(Show show);
    Task DeleteAsync(Show show);
    Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int excludeShowId = 0);
}
