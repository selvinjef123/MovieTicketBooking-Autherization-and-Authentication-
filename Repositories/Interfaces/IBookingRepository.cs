using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces;

public interface IBookingRepository
{
    Task<IEnumerable<Booking>> GetAllAsync();
    Task<Booking?> GetByIdAsync(int id);
    Task<Booking?> GetByIdForUpdateAsync(int id);
    Task<IEnumerable<Booking>> GetByShowIdAsync(int showId);
    Task<IEnumerable<Booking>> GetByUserIdAsync(int userId);
    Task<Booking> CreateBookingAsync(Booking booking, IEnumerable<BookingSeat> bookingSeats, IEnumerable<ShowSeat> showSeatsToUpdate);
    Task CancelBookingAsync(Booking booking, IEnumerable<ShowSeat> showSeatsToRelease);
}
