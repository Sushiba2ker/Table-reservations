using TableReservations.Common.Results;
using TableReservations.DTOs;
using TableReservations.Models;
using TableReservations.Repositories;
using TableReservations.Services.Interfaces;

namespace TableReservations.Services;

/// <summary>
/// Table location service implementation containing business logic for table management
/// </summary>
public class TableLocationService : ITableLocationService
{
    private readonly ITableLocationRepository _tableLocationRepository;
    private readonly IBookingRepository _bookingRepository;

    public TableLocationService(ITableLocationRepository tableLocationRepository, IBookingRepository bookingRepository)
    {
        _tableLocationRepository = tableLocationRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<Result<IEnumerable<TableLocationDto>>> GetAllTableLocationsAsync()
    {
        try
        {
            var tableLocations = await _tableLocationRepository.GetAllAsync();
            
            var tableLocationDtos = tableLocations.Select(tl => new TableLocationDto
            {
                Id = tl.Id,
                Name = tl.Name,
                ImageUrl = tl.ImageUrl,
                IsAvailable = true // Default to available, can be calculated based on current bookings
            }).ToList();

            return Result<IEnumerable<TableLocationDto>>.Success(tableLocationDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TableLocationDto>>.Failure($"Error retrieving table locations: {ex.Message}", 500);
        }
    }

    public async Task<Result<TableLocationDto>> GetTableLocationByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<TableLocationDto>.Failure("Invalid table location ID", 400);
            }

            var tableLocation = await _tableLocationRepository.GetByIdAsync(id);
            
            if (tableLocation == null)
            {
                return Result<TableLocationDto>.NotFound($"Table location with ID {id} not found");
            }

            var tableLocationDto = new TableLocationDto
            {
                Id = tableLocation.Id,
                Name = tableLocation.Name,
                ImageUrl = tableLocation.ImageUrl,
                IsAvailable = true // Can be calculated based on current bookings
            };

            return Result<TableLocationDto>.Success(tableLocationDto);
        }
        catch (Exception ex)
        {
            return Result<TableLocationDto>.Failure($"Error retrieving table location: {ex.Message}", 500);
        }
    }

    public async Task<Result<TableLocationDto>> CreateTableLocationAsync(CreateUpdateTableLocationDto tableLocationDto)
    {
        try
        {
            // Check if table location with same name already exists
            var existingTableLocations = await _tableLocationRepository.GetAllAsync();
            var existingTableLocation = existingTableLocations.FirstOrDefault(tl => 
                tl.Name.Equals(tableLocationDto.Name, StringComparison.OrdinalIgnoreCase));
            
            if (existingTableLocation != null)
            {
                return Result<TableLocationDto>.Failure("Table location with this name already exists", 400);
            }

            var tableLocation = new TableLocation
            {
                Name = tableLocationDto.Name,
                ImageUrl = tableLocationDto.ImageUrl
            };

            await _tableLocationRepository.AddAsync(tableLocation);

            var createdTableLocationDto = new TableLocationDto
            {
                Id = tableLocation.Id,
                Name = tableLocation.Name,
                ImageUrl = tableLocation.ImageUrl,
                IsAvailable = true
            };

            return Result<TableLocationDto>.Success(createdTableLocationDto, 201);
        }
        catch (Exception ex)
        {
            return Result<TableLocationDto>.Failure($"Error creating table location: {ex.Message}", 500);
        }
    }

    public async Task<Result<TableLocationDto>> UpdateTableLocationAsync(int id, CreateUpdateTableLocationDto tableLocationDto)
    {
        try
        {
            if (id <= 0)
            {
                return Result<TableLocationDto>.Failure("Invalid table location ID", 400);
            }

            var existingTableLocation = await _tableLocationRepository.GetByIdAsync(id);
            if (existingTableLocation == null)
            {
                return Result<TableLocationDto>.NotFound($"Table location with ID {id} not found");
            }

            // Check if another table location with same name already exists
            var allTableLocations = await _tableLocationRepository.GetAllAsync();
            var duplicateTableLocation = allTableLocations.FirstOrDefault(tl => 
                tl.Id != id && tl.Name.Equals(tableLocationDto.Name, StringComparison.OrdinalIgnoreCase));
            
            if (duplicateTableLocation != null)
            {
                return Result<TableLocationDto>.Failure("Another table location with this name already exists", 400);
            }

            // Update table location properties
            existingTableLocation.Name = tableLocationDto.Name;
            existingTableLocation.ImageUrl = tableLocationDto.ImageUrl;

            await _tableLocationRepository.UpdateAsync(existingTableLocation);

            var updatedTableLocationDto = new TableLocationDto
            {
                Id = existingTableLocation.Id,
                Name = existingTableLocation.Name,
                ImageUrl = existingTableLocation.ImageUrl,
                IsAvailable = true
            };

            return Result<TableLocationDto>.Success(updatedTableLocationDto);
        }
        catch (Exception ex)
        {
            return Result<TableLocationDto>.Failure($"Error updating table location: {ex.Message}", 500);
        }
    }

    public async Task<Result> DeleteTableLocationAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid table location ID", 400);
            }

            var tableLocation = await _tableLocationRepository.GetByIdAsync(id);
            if (tableLocation == null)
            {
                return Result.NotFound($"Table location with ID {id} not found");
            }

            // Check if table has active bookings
            var bookings = await _bookingRepository.GetAllAsync();
            var hasActiveBookings = bookings.Any(b => 
                b.TableLocation.Equals(tableLocation.Name, StringComparison.OrdinalIgnoreCase) &&
                b.DateTime >= DateTime.Now);

            if (hasActiveBookings)
            {
                return Result.Failure("Cannot delete table location with active bookings", 400);
            }

            await _tableLocationRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting table location: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> IsTableLocationAvailableAsync(int tableLocationId, DateTime dateTime, int durationHours = 2)
    {
        try
        {
            if (tableLocationId <= 0)
            {
                return Result<bool>.Failure("Invalid table location ID", 400);
            }

            var tableLocation = await _tableLocationRepository.GetByIdAsync(tableLocationId);
            if (tableLocation == null)
            {
                return Result<bool>.Success(false); // Non-existent table is not available
            }

            if (dateTime <= DateTime.Now)
            {
                return Result<bool>.Success(false); // Past dates are not available
            }

            var bookings = await _bookingRepository.GetAllAsync();
            var endTime = dateTime.AddHours(durationHours);
            
            var isAvailable = !bookings.Any(b => 
                b.TableLocation.Equals(tableLocation.Name, StringComparison.OrdinalIgnoreCase) &&
                !(b.DateTime.AddHours(2) <= dateTime || // Booking ends before our start
                  b.DateTime >= endTime)); // Booking starts after our end

            return Result<bool>.Success(isAvailable);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking table availability: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<TableLocationDto>>> GetAvailableTableLocationsAsync(DateTime dateTime, int durationHours = 2)
    {
        try
        {
            var allTableLocations = await _tableLocationRepository.GetAllAsync();
            var availableTableLocations = new List<TableLocationDto>();

            foreach (var tableLocation in allTableLocations)
            {
                var isAvailable = await IsTableLocationAvailableAsync(tableLocation.Id, dateTime, durationHours);
                if (isAvailable.IsSuccess && isAvailable.Data)
                {
                    availableTableLocations.Add(new TableLocationDto
                    {
                        Id = tableLocation.Id,
                        Name = tableLocation.Name,
                        ImageUrl = tableLocation.ImageUrl,
                        IsAvailable = true
                    });
                }
            }

            return Result<IEnumerable<TableLocationDto>>.Success(availableTableLocations);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TableLocationDto>>.Failure($"Error retrieving available table locations: {ex.Message}", 500);
        }
    }

    public async Task<Result<object>> GetTableLocationStatisticsAsync()
    {
        try
        {
            var tableLocations = await _tableLocationRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();
            
            var statistics = new
            {
                TotalTableLocations = tableLocations.Count(),
                TotalBookings = bookings.Count(),
                TodayBookings = bookings.Count(b => b.DateTime.Date == DateTime.Today),
                PopularTableLocations = bookings.GroupBy(b => b.TableLocation)
                                               .Select(g => new { TableLocation = g.Key, BookingCount = g.Count() })
                                               .OrderByDescending(x => x.BookingCount)
                                               .Take(5)
                                               .ToList(),
                AverageBookingsPerTable = tableLocations.Any() ? 
                    bookings.Count() / (double)tableLocations.Count() : 0
            };

            return Result<object>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Error retrieving table location statistics: {ex.Message}", 500);
        }
    }

    // Add TableLocationExistsAsync method as required by interface
    public async Task<Result<bool>> TableLocationExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<bool>.Success(false);
            }

            var tableLocation = await _tableLocationRepository.GetByIdAsync(id);
            return Result<bool>.Success(tableLocation != null);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking table location existence: {ex.Message}", 500);
        }
    }

    // Add GetAvailableTablesAsync method as required by interface
    public async Task<Result<IEnumerable<TableLocationDto>>> GetAvailableTablesAsync(DateTime reservationDate, int numberOfGuests)
    {
        try
        {
            var tableLocations = await _tableLocationRepository.GetAllAsync();
            var availableTableLocations = new List<TableLocationDto>();

            foreach (var tableLocation in tableLocations)
            {
                var isAvailable = await IsTableLocationAvailableAsync(tableLocation.Id, reservationDate, 2);
                if (isAvailable.IsSuccess && isAvailable.Data)
                {
                    availableTableLocations.Add(new TableLocationDto
                    {
                        Id = tableLocation.Id,
                        Name = tableLocation.Name,
                        ImageUrl = tableLocation.ImageUrl,
                        IsAvailable = true
                    });
                }
            }

            return Result<IEnumerable<TableLocationDto>>.Success(availableTableLocations);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TableLocationDto>>.Failure($"Error retrieving available tables: {ex.Message}", 500);
        }
    }
} 