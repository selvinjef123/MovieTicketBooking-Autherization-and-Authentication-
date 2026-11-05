namespace MovieTicketBooking.DTOs.Theatres;

public class CreateTheatreRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class UpdateTheatreRequest
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class TheatreResponse
{
    public int TheatreId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
