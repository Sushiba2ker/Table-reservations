using System.Collections.Generic;
using BT3_TH.Models;

namespace BT3_TH.Repositories
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