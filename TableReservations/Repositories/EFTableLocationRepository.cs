using System.Collections.Generic;
using System.Threading.Tasks;
using TableReservations.Models;
using Microsoft.EntityFrameworkCore;

namespace TableReservations.Repositories
{
    public class EFTableLocationRepository : ITableLocationRepository
    {
        private readonly ApplicationDbContext _context;

        public EFTableLocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TableLocation>> GetAllAsync()
        {
            return await _context.TableLocations.ToListAsync();
        }

        public async Task<TableLocation?> GetByIdAsync(int id)
        {
            return await _context.TableLocations.FindAsync(id);
        }

        public async Task AddAsync(TableLocation tableLocation)
        {
            _context.TableLocations.Add(tableLocation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TableLocation tableLocation)
        {
            _context.TableLocations.Update(tableLocation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tableLocation = await _context.TableLocations.FindAsync(id);
            if (tableLocation != null)
            {
                _context.TableLocations.Remove(tableLocation);
                await _context.SaveChangesAsync();
            }
        }
    }
}