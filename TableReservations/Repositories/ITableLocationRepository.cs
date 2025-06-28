using System.Collections.Generic;
using TableReservations.Models;

namespace TableReservations.Repositories
{
    public interface ITableLocationRepository
    {
        Task<IEnumerable<TableLocation>> GetAllAsync();
        Task<TableLocation?> GetByIdAsync(int id);
        Task AddAsync(TableLocation tableLocation);
        Task UpdateAsync(TableLocation tableLocation);
        Task DeleteAsync(int id);
    }
}