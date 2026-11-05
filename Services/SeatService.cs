using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;
    public SeatService(ISeatRepository seatRepository) => _seatRepository = seatRepository;

    public async Task<IEnumerable<SeatResponse>> GetSeatsByScreenIdAsync(int screenId)
    {
        var seats = await _seatRepository.GetByScreenIdAsync(screenId);
        return seats.Select(s => new SeatResponse { SeatId = s.SeatId, ScreenId = s.ScreenId, RowName = s.RowName, SeatNumber = s.SeatNumber, SeatType = s.SeatType });
    }

    public async Task<SeatResponse?> GetSeatByIdAsync(int id)
    {
        var s = await _seatRepository.GetByIdAsync(id);
        if (s == null) return null;
        return new SeatResponse { SeatId = s.SeatId, ScreenId = s.ScreenId, RowName = s.RowName, SeatNumber = s.SeatNumber, SeatType = s.SeatType };
    }

    public async Task<SeatResponse> CreateSeatAsync(CreateSeatRequest request)
    {
        var seat = new Seat { ScreenId = request.ScreenId, RowName = request.RowName, SeatNumber = request.SeatNumber, SeatType = request.SeatType };
        var created = await _seatRepository.AddAsync(seat);
        return new SeatResponse { SeatId = created.SeatId, ScreenId = created.ScreenId, RowName = created.RowName, SeatNumber = created.SeatNumber, SeatType = created.SeatType };
    }

    public async Task<IEnumerable<SeatResponse>> BulkCreateSeatsAsync(BulkCreateSeatsRequest request)
    {
        var seatsList = new List<Seat>();
        for (int i = request.StartNumber; i <= request.EndNumber; i++)
        {
            seatsList.Add(new Seat
            {
                ScreenId = request.ScreenId,
                RowName = request.RowName,
                SeatNumber = i,
                SeatType = request.SeatType
            });
        }

        await _seatRepository.AddRangeAsync(seatsList);
        return seatsList.Select(s => new SeatResponse { SeatId = s.SeatId, ScreenId = s.ScreenId, RowName = s.RowName, SeatNumber = s.SeatNumber, SeatType = s.SeatType });
    }

    public async Task<bool> UpdateSeatAsync(int id, UpdateSeatRequest request)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return false;
        seat.RowName = request.RowName;
        seat.SeatNumber = request.SeatNumber;
        seat.SeatType = request.SeatType;
        await _seatRepository.UpdateAsync(seat);
        return true;
    }

    public async Task<bool> DeleteSeatAsync(int id)
    {
        var seat = await _seatRepository.GetByIdAsync(id);
        if (seat == null) return false;
        try
        {
            await _seatRepository.DeleteAsync(seat);
            return true;
        }
        catch { return false; }
    }
}
