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

    public async Task<IEnumerable<ScreenResponse>> GetAllScreensAsync()
    {
        var screens = await _screenRepository.GetAllAsync();
        return screens.Select(s => new ScreenResponse { ScreenId = s.ScreenId, TheatreId = s.TheatreId, TheatreName = s.Theatre?.Name ?? string.Empty, ScreenName = s.ScreenName });
    }

    public async Task<ScreenResponse?> GetScreenByIdAsync(int id)
    {
        var s = await _screenRepository.GetByIdAsync(id);
        if (s == null) return null;
        return new ScreenResponse { ScreenId = s.ScreenId, TheatreId = s.TheatreId, TheatreName = s.Theatre?.Name ?? string.Empty, ScreenName = s.ScreenName };
    }

    public async Task<IEnumerable<ScreenResponse>> GetScreensByTheatreIdAsync(int theatreId)
    {
        var screens = await _screenRepository.GetByTheatreIdAsync(theatreId);
        return screens.Select(s => new ScreenResponse { ScreenId = s.ScreenId, TheatreId = s.TheatreId, ScreenName = s.ScreenName });
    }

    public async Task<ScreenResponse> CreateScreenAsync(CreateScreenRequest request)
    {
        var screen = new Screen { TheatreId = request.TheatreId, ScreenName = request.ScreenName };
        var created = await _screenRepository.AddAsync(screen);
        return new ScreenResponse { ScreenId = created.ScreenId, TheatreId = created.TheatreId, ScreenName = created.ScreenName };
    }

    public async Task<bool> UpdateScreenAsync(int id, UpdateScreenRequest request)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null) return false;
        screen.ScreenName = request.ScreenName;
        await _screenRepository.UpdateAsync(screen);
        return true;
    }

    public async Task<bool> DeleteScreenAsync(int id)
    {
        var screen = await _screenRepository.GetByIdAsync(id);
        if (screen == null) return false;
        try
        {
            await _screenRepository.DeleteAsync(screen);
            return true;
        }
        catch { return false; }
    }
}
