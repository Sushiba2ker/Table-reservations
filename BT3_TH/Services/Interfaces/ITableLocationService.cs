using BT3_TH.Common.Results;
using BT3_TH.DTOs;

namespace BT3_TH.Services.Interfaces;

/// <summary>
/// Service interface for Table Location business logic
/// </summary>
public interface ITableLocationService
{
    /// <summary>
    /// Get all table locations
    /// </summary>
    Task<Result<IEnumerable<TableLocationDto>>> GetAllTableLocationsAsync();
    
    /// <summary>
    /// Get table location by ID
    /// </summary>
    Task<Result<TableLocationDto>> GetTableLocationByIdAsync(int id);
    
    /// <summary>
    /// Create new table location
    /// </summary>
    Task<Result<TableLocationDto>> CreateTableLocationAsync(CreateUpdateTableLocationDto tableLocationDto);
    
    /// <summary>
    /// Update existing table location
    /// </summary>
    Task<Result<TableLocationDto>> UpdateTableLocationAsync(int id, CreateUpdateTableLocationDto tableLocationDto);
    
    /// <summary>
    /// Delete table location
    /// </summary>
    Task<Result> DeleteTableLocationAsync(int id);
    
    /// <summary>
    /// Get available table locations for specific date/time
    /// </summary>
    Task<Result<IEnumerable<TableLocationDto>>> GetAvailableTablesAsync(DateTime dateTime, int duration = 2);
    
    /// <summary>
    /// Check if table location exists
    /// </summary>
    Task<Result<bool>> TableLocationExistsAsync(int id);
} 