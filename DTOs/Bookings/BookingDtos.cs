using System.ComponentModel.DataAnnotations;

namespace MovieTicketBooking.DTOs.Bookings;

public class CreateBookingRequest
{
    [Required]
    [MaxLength(15)]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    public int ShowId { get; set; }

    [Required]
    public List<int> SeatIds { get; set; } = new();
}

public class BookingResponse
{
    public int BookingId { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public int ShowId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public string TheatreName { get; set; } = string.Empty;
    public string ScreenName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty; // Confirmed or Cancelled
    public List<string> Seats { get; set; } = new(); // e.g., ["A1", "A2"]
}
