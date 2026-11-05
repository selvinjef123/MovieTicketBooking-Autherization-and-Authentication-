using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ShowRepository : IShowRepository
{
    private readonly ApplicationDbContext _context;
    public ShowRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Show>> GetAllAsync() =>
        await _context.Shows.Include(s => s.Movie).Include(s => s.Screen).ThenInclude(sc => sc.Theatre).ToListAsync();

    public async Task<Show?> GetByIdAsync(int id) =>
        await _context.Shows.Include(s => s.Movie).Include(s => s.Screen).ThenInclude(sc => sc.Theatre).FirstOrDefaultAsync(s => s.ShowId == id);

    public async Task<IEnumerable<Show>> GetByMovieIdAsync(int movieId) =>
        await _context.Shows.Include(s => s.Movie).Include(s => s.Screen).Where(s => s.MovieId == movieId).ToListAsync();

    public async Task<IEnumerable<Show>> GetByTheatreIdAsync(int theatreId) =>
        await _context.Shows.Include(s => s.Movie).Include(s => s.Screen).Where(s => s.Screen.TheatreId == theatreId).ToListAsync();

    public async Task<IEnumerable<ShowSeat>> GetShowSeatsAsync(int showId) =>
        await _context.ShowSeats.Include(ss => ss.Seat).Where(ss => ss.ShowId == showId).ToListAsync();

    public async Task<ShowSeat?> GetShowSeatAsync(int showId, int seatId) =>
        await _context.ShowSeats.Include(ss => ss.Seat).FirstOrDefaultAsync(ss => ss.ShowId == showId && ss.SeatId == seatId);

    public async Task<Show> AddAsync(Show show, IEnumerable<ShowSeat> showSeats)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Shows.Add(show);
                await _context.SaveChangesAsync();

                foreach (var seat in showSeats)
                {
                    seat.ShowId = show.ShowId;
                }
                _context.ShowSeats.AddRange(showSeats);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return show;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task UpdateAsync(Show show)
    {
        _context.Shows.Update(show);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Show show)
    {
        _context.Shows.Remove(show);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasOverlappingShowAsync(int screenId, DateTime startTime, DateTime endTime, int excludeShowId = 0)
    {
        return await _context.Shows.AnyAsync(s => s.ScreenId == screenId &&
            s.ShowId != excludeShowId &&
            s.StartTime < endTime && s.EndTime > startTime);
    }
}
