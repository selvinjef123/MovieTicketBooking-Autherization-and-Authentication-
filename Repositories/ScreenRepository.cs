using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories;

public class ScreenRepository : IScreenRepository
{
	private readonly ApplicationDbContext _context;
	public ScreenRepository(ApplicationDbContext context) => _context = context;

	public async Task<IEnumerable<Screen>> GetAllAsync() => await _context.Screens.Include(s => s.Theatre).ToListAsync();
	public async Task<Screen?> GetByIdAsync(int id) => await _context.Screens.Include(s => s.Theatre).FirstOrDefaultAsync(s => s.ScreenId == id);
	public async Task<IEnumerable<Screen>> GetByTheatreIdAsync(int theatreId) => await _context.Screens.Where(s => s.TheatreId == theatreId).ToListAsync();
	public async Task<Screen> AddAsync(Screen screen)
	{
		_context.Screens.Add(screen);
		await _context.SaveChangesAsync();
		return screen;
	}
	public async Task UpdateAsync(Screen screen)
	{
		_context.Screens.Update(screen);
		await _context.SaveChangesAsync();
	}
	public async Task DeleteAsync(Screen screen)
	{
		_context.Screens.Remove(screen);
		await _context.SaveChangesAsync();
	}
}
