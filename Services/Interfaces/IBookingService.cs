using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces;

public interface IBookingService
{
	Task<IEnumerable<BookingResponse>> GetAllBookingsAsync();
	Task<BookingResponse?> GetBookingByIdAsync(int id, int? userId = null);
	Task<IEnumerable<BookingResponse>> GetBookingsByShowIdAsync(int showId);
	Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(int userId);
	Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId, string customerName, string customerEmail);
	Task<(bool Success, string? Error)> CancelBookingAsync(int id, int? userId = null);
}
