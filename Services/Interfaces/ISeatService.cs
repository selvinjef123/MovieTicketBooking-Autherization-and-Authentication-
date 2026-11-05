using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces;

public interface ISeatService
{
	Task<IEnumerable<SeatResponse>> GetSeatsByScreenIdAsync(int screenId);
	Task<SeatResponse?> GetSeatByIdAsync(int id);
	Task<SeatResponse> CreateSeatAsync(CreateSeatRequest request);
	Task<IEnumerable<SeatResponse>> BulkCreateSeatsAsync(BulkCreateSeatsRequest request);
	Task<bool> UpdateSeatAsync(int id, UpdateSeatRequest request);
	Task<bool> DeleteSeatAsync(int id);
}
