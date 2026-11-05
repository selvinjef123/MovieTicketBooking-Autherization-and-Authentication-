using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IShowRepository _showRepository;

    public BookingService(IBookingRepository bookingRepository, IShowRepository showRepository)
    {
        _bookingRepository = bookingRepository;
        _showRepository = showRepository;
    }

    public async Task<IEnumerable<BookingResponse>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return bookings.Select(MapToBookingResponse);
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(int id, int? userId = null)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null) return null;
        if (userId.HasValue && booking.UserId != userId.Value) return null;
        return MapToBookingResponse(booking);
    }

    public async Task<IEnumerable<BookingResponse>> GetBookingsByShowIdAsync(int showId)
    {
        var bookings = await _bookingRepository.GetByShowIdAsync(showId);
        return bookings.Select(MapToBookingResponse);
    }

    public async Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(int userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        return bookings.Select(MapToBookingResponse);
    }

    public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request, int userId, string customerName, string customerEmail)
    {
        var show = await _showRepository.GetByIdAsync(request.ShowId);
        if (show == null) throw new KeyNotFoundException("Show not found.");

        // Verify distinct seat IDs
        var distinctSeatIds = request.SeatIds?.Distinct().ToList() ?? new List<int>();
        if (!distinctSeatIds.Any()) throw new ArgumentException("Choose at least one seat.");

        var showSeatsToUpdate = new List<ShowSeat>();
        var bookingSeats = new List<BookingSeat>();
        decimal totalAmount = 0;

        foreach (var seatId in distinctSeatIds)
        {
            var showSeat = await _showRepository.GetShowSeatAsync(request.ShowId, seatId);
            if (showSeat == null) throw new InvalidOperationException($"Seat {seatId} does not belong to this show.");
            if (showSeat.Status != "Available") throw new InvalidOperationException($"Seat {showSeat.Seat.RowName}{showSeat.Seat.SeatNumber} is already booked.");

            showSeatsToUpdate.Add(showSeat);

            decimal seatPrice = show.TicketPrice;
            totalAmount += seatPrice;

            bookingSeats.Add(new BookingSeat
            {
                SeatId = seatId,
                Price = seatPrice
            });
        }

        var tempReference = $"MOV-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        var booking = new Booking
        {
            BookingReference = tempReference,
            ShowId = request.ShowId,
            UserId = userId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = request.CustomerPhone,
            BookingDate = DateTime.UtcNow,
            TotalAmount = totalAmount,
            Status = "Confirmed"
        };

        var createdBooking = await _bookingRepository.CreateBookingAsync(booking, bookingSeats, showSeatsToUpdate);

        var fullBooking = await _bookingRepository.GetByIdAsync(createdBooking.BookingId);
        return MapToBookingResponse(fullBooking!);
    }

    public async Task<(bool Success, string? Error)> CancelBookingAsync(int id, int? userId = null)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null) return (false, "Booking not found.");
        if (userId.HasValue && booking.UserId != userId.Value) return (false, "You cannot cancel another customer's booking.");
        if (booking.Status == "Cancelled") return (false, "Booking is already cancelled.");

        var showSeatsToRelease = new List<ShowSeat>();
        foreach (var bs in booking.BookingSeats)
        {
            var showSeat = await _showRepository.GetShowSeatAsync(booking.ShowId, bs.SeatId);
            if (showSeat != null) showSeatsToRelease.Add(showSeat);
        }

        await _bookingRepository.CancelBookingAsync(booking, showSeatsToRelease);
        return (true, null);
    }

    private static BookingResponse MapToBookingResponse(Booking b) => new()
    {
        BookingId = b.BookingId,
        BookingReference = b.BookingReference,
        ShowId = b.ShowId,
        MovieTitle = b.Show?.Movie?.Title ?? string.Empty,
        TheatreName = b.Show?.Screen?.Theatre?.Name ?? string.Empty,
        ScreenName = b.Show?.Screen?.ScreenName ?? string.Empty,
        CustomerName = b.CustomerName,
        CustomerEmail = b.CustomerEmail,
        CustomerPhone = b.CustomerPhone,
        BookingDate = b.BookingDate,
        TotalAmount = b.TotalAmount,
        Status = b.Status,
        Seats = b.BookingSeats?.Select(bs => $"{bs.Seat?.RowName}{bs.Seat?.SeatNumber}").ToList() ?? new()
    };
}
