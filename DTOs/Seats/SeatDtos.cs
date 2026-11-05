namespace MovieTicketBooking.DTOs.Seats;

public class CreateSeatRequest
{
    public int ScreenId { get; set; }
    public string RowName { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = string.Empty; // e.g., Regular, Premium
}

public class BulkCreateSeatsRequest
{
    public int ScreenId { get; set; }
    public string RowName { get; set; } = string.Empty;
    public int StartNumber { get; set; }
    public int EndNumber { get; set; }
    public string SeatType { get; set; } = "Regular";
}

public class UpdateSeatRequest
{
    public string RowName { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = string.Empty;
}

public class SeatResponse
{
    public int SeatId { get; set; }
    public int ScreenId { get; set; }
    public string RowName { get; set; } = string.Empty;
    public int SeatNumber { get; set; }
    public string SeatType { get; set; } = string.Empty;
}
