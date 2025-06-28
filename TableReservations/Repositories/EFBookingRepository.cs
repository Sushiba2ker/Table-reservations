using TableReservations.Models;
using Microsoft.EntityFrameworkCore;

namespace TableReservations.Repositories;

public class EFBookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public EFBookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(b => b.TableLocation)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(b => b.TableLocation)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var booking = await GetByIdAsync(id);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
