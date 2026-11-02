using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class ScreenService : IScreenService
{
    private readonly IScreenRepository _screenRepository;
    private readonly ITheatreRepository _theatreRepository;

    public ScreenService(IScreenRepository screenRepository, ITheatreRepository theatreRepository)
    {
        _screenRepository = screenRepository;
        _theatreRepository = theatreRepository;
    }

    public async Task<IEnumerable<ScreenResponse>> GetAllAsync(int? theatreId)
    {
        var screens = await _screenRepository.GetAllAsync(theatreId);
        return screens.Select(MapToResponse);
    }

    public async Task<ScreenResponse?> GetByIdAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        return screen == null ? null : MapToResponse(screen);
    }

    public async Task<(ScreenResponse? Response, string? ErrorMessage)> CreateAsync(CreateScreenRequest request)
    {
        var theatre = await _theatreRepository.GetByIdAsync(request.TheatreId);
        if (theatre == null)
            return (null, $"Theatre with ID {request.TheatreId} does not exist.");

        var screen = new Screen
        {
            TheatreId = request.TheatreId,
            ScreenName = request.ScreenName
        };

        var created = await _screenRepository.CreateAsync(screen);

        // Map safely using scalar values to avoid triggering EF Core navigation cycles/nulls
        var response = new ScreenResponse(
            created.ScreenId,
            created.TheatreId,
            created.ScreenName,
            theatre.Name
        );

        return (response, null);
    }

    public async Task<bool> UpdateAsync(int id, UpdateScreenRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null) return false;

        screen.ScreenName = request.ScreenName;
        await _screenRepository.UpdateAsync(screen);
        return true;
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null) return (false, null);

        if (await _screenRepository.HasSeatsOrShowsAsync(id))
            return (false, "Cannot delete screen with configured physical seats or associated shows.");

        await _screenRepository.DeleteAsync(screen);
        return (true, null);
    }

    private static ScreenResponse MapToResponse(Screen s) =>
        new(
            s.ScreenId,
            s.TheatreId,
            s.ScreenName,
            s.Theatre != null ? s.Theatre.Name : string.Empty
        );
}