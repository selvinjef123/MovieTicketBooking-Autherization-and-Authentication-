using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;
    public BookingRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Booking>> GetAllAsync() =>
        await _context.Bookings
            .Include(b => b.Show).ThenInclude(s => s.Movie)
            .Include(b => b.Show).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

    public async Task<Booking?> GetByIdAsync(int id) =>
        await _context.Bookings
            .Include(b => b.Show).ThenInclude(s => s.Movie)
            .Include(b => b.Show).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .FirstOrDefaultAsync(b => b.BookingId == id);

    public async Task<Booking?> GetByIdForUpdateAsync(int id) =>
        await _context.Bookings
            .Include(b => b.BookingSeats)
            .FirstOrDefaultAsync(b => b.BookingId == id);

    public async Task<IEnumerable<Booking>> GetByShowIdAsync(int showId) =>
        await _context.Bookings
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .Where(b => b.ShowId == showId)
            .ToListAsync();

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId) =>
        await _context.Bookings
            .Include(b => b.Show).ThenInclude(s => s.Movie)
            .Include(b => b.Show).ThenInclude(s => s.Screen).ThenInclude(sc => sc.Theatre)
            .Include(b => b.BookingSeats).ThenInclude(bs => bs.Seat)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();

    public async Task<Booking> CreateBookingAsync(Booking booking, IEnumerable<BookingSeat> bookingSeats, IEnumerable<ShowSeat> showSeatsToUpdate)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                booking.BookingReference = $"MOV{booking.BookingId}";

                foreach (var bs in bookingSeats)
                {
                    bs.BookingId = booking.BookingId;
                }
                _context.BookingSeats.AddRange(bookingSeats);

                foreach (var ss in showSeatsToUpdate)
                {
                    ss.Status = "Booked";
                    _context.ShowSeats.Update(ss);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task CancelBookingAsync(Booking booking, IEnumerable<ShowSeat> showSeatsToRelease)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                booking.Status = "Cancelled";
                _context.Bookings.Update(booking);

                foreach (var ss in showSeatsToRelease)
                {
                    ss.Status = "Available";
                    _context.ShowSeats.Update(ss);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
