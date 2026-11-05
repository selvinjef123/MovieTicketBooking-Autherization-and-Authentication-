using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;
    public SeatRepository(ApplicationDbContext context) => _context = context;

    public async Task<IEnumerable<Seat>> GetByScreenIdAsync(int screenId) => await _context.Seats.Where(s => s.ScreenId == screenId).ToListAsync();
    public async Task<Seat?> GetByIdAsync(int id) => await _context.Seats.FindAsync(id);
    public async Task<Seat> AddAsync(Seat seat)
    {
        _context.Seats.Add(seat);
        await _context.SaveChangesAsync();
        return seat;
    }
    public async Task AddRangeAsync(IEnumerable<Seat> seats)
    {
        _context.Seats.AddRange(seats);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Seat seat)
    {
        _context.Seats.Update(seat);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Seat seat)
    {
        _context.Seats.Remove(seat);
        await _context.SaveChangesAsync();
    }
}
