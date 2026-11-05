namespace MovieTicketBooking.DTOs.Shows;

public class CreateShowRequest
{
    public int MovieId { get; set; }
    public int ScreenId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TicketPrice { get; set; }
}

public class UpdateShowRequest
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TicketPrice { get; set; }
}

public class ShowResponse
{
    public int ShowId { get; set; }
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public int ScreenId { get; set; }
    public string ScreenName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal TicketPrice { get; set; }
}

public class ShowSeatResponse
{
    public int SeatId { get; set; }
    public string RowName { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatLabel => $"{RowName}{SeatNumber}";
    public string SeatType { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty; // Available or Booked
}
