using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ShowService : IShowService
{
    private readonly IShowRepository _showRepository;
    private readonly ISeatRepository _seatRepository;

    public ShowService(IShowRepository showRepository, ISeatRepository seatRepository)
    {
        _showRepository = showRepository;
        _seatRepository = seatRepository;
    }

    public async Task<IEnumerable<ShowResponse>> GetAllShowsAsync()
    {
        var shows = await _showRepository.GetAllAsync();
        return shows.Select(MapToShowResponse);
    }

    public async Task<ShowResponse?> GetShowByIdAsync(int id)
    {
        var show = await _showRepository.GetByIdAsync(id);
        return show == null ? null : MapToShowResponse(show);
    }

    public async Task<IEnumerable<ShowResponse>> GetShowsByMovieIdAsync(int movieId)
    {
        var shows = await _showRepository.GetByMovieIdAsync(movieId);
        return shows.Select(MapToShowResponse);
    }

    public async Task<IEnumerable<ShowResponse>> GetShowsByTheatreIdAsync(int theatreId)
    {
        var shows = await _showRepository.GetByTheatreIdAsync(theatreId);
        return shows.Select(MapToShowResponse);
    }

    public async Task<IEnumerable<ShowSeatResponse>> GetShowSeatsAsync(int showId)
    {
        var showSeats = await _showRepository.GetShowSeatsAsync(showId);
        var show = await _showRepository.GetByIdAsync(showId);
        decimal price = show?.TicketPrice ?? 0;

        return showSeats.Select(ss => new ShowSeatResponse
        {
            SeatId = ss.SeatId,
            RowName = ss.Seat.RowName,
            SeatNumber = ss.Seat.SeatNumber,
            SeatType = ss.Seat.SeatType,
            Price = price,
            Status = ss.Status
        });
    }

    public async Task<ShowResponse> CreateShowAsync(CreateShowRequest request)
    {
        // Validate overlap
        if (await _showRepository.HasOverlappingShowAsync(request.ScreenId, request.StartTime, request.EndTime))
        {
            throw new InvalidOperationException("The screen is already scheduled for an overlapping show.");
        }

        var show = new Show
        {
            MovieId = request.MovieId,
            ScreenId = request.ScreenId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            TicketPrice = request.TicketPrice
        };

        // Retrieve physical seats for this screen to generate ShowSeats automatically
        var physicalSeats = await _seatRepository.GetByScreenIdAsync(request.ScreenId);
        var showSeats = physicalSeats.Select(seat => new ShowSeat
        {
            SeatId = seat.SeatId,
            Status = "Available"
        }).ToList();

        var created = await _showRepository.AddAsync(show, showSeats);
        var fullShow = await _showRepository.GetByIdAsync(created.ShowId);
        return MapToShowResponse(fullShow!);
    }

    public async Task<bool> UpdateShowAsync(int id, UpdateShowRequest request)
    {
        var show = await _showRepository.GetByIdAsync(id);
        if (show == null) return false;

        if (await _showRepository.HasOverlappingShowAsync(show.ScreenId, request.StartTime, request.EndTime, id))
        {
            throw new InvalidOperationException("The screen is already scheduled for an overlapping show.");
        }

        show.StartTime = request.StartTime;
        show.EndTime = request.EndTime;
        show.TicketPrice = request.TicketPrice;

        await _showRepository.UpdateAsync(show);
        return true;
    }

    public async Task<bool> DeleteShowAsync(int id)
    {
        var show = await _showRepository.GetByIdAsync(id);
        if (show == null) return false;
        try
        {
            await _showRepository.DeleteAsync(show);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static ShowResponse MapToShowResponse(Show s) => new()
    {
        ShowId = s.ShowId,
        MovieId = s.MovieId,
        MovieTitle = s.Movie?.Title ?? string.Empty,
        ScreenId = s.ScreenId,
        ScreenName = s.Screen?.ScreenName ?? string.Empty,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        TicketPrice = s.TicketPrice
    };
}
