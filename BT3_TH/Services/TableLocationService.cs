using BT3_TH.Common.Results;
using BT3_TH.DTOs;
using BT3_TH.Models;
using BT3_TH.Repositories;
using BT3_TH.Services.Interfaces;

namespace BT3_TH.Services;

/// <summary>
/// Table location service implementation containing business logic
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
                Capacity = tl.Capacity,
                IsAvailable = true // Will be determined based on current bookings
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
                Capacity = tableLocation.Capacity,
                IsAvailable = true
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
            // Check if table location name already exists
            var existingTableLocations = await _tableLocationRepository.GetAllAsync();
            if (existingTableLocations.Any(tl => tl.Name.Equals(tableLocationDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<TableLocationDto>.Failure($"Table location with name '{tableLocationDto.Name}' already exists", 400);
            }

            var tableLocation = new TableLocation
            {
                Name = tableLocationDto.Name,
                Capacity = tableLocationDto.Capacity
            };

            await _tableLocationRepository.AddAsync(tableLocation);

            var createdTableLocationDto = new TableLocationDto
            {
                Id = tableLocation.Id,
                Name = tableLocation.Name,
                Capacity = tableLocation.Capacity,
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

            // Check if new name conflicts with existing table locations (excluding current one)
            var existingTableLocations = await _tableLocationRepository.GetAllAsync();
            if (existingTableLocations.Any(tl => tl.Id != id && tl.Name.Equals(tableLocationDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Result<TableLocationDto>.Failure($"Table location with name '{tableLocationDto.Name}' already exists", 400);
            }

            existingTableLocation.Name = tableLocationDto.Name;
            existingTableLocation.Capacity = tableLocationDto.Capacity;
            await _tableLocationRepository.UpdateAsync(existingTableLocation);

            var updatedTableLocationDto = new TableLocationDto
            {
                Id = existingTableLocation.Id,
                Name = existingTableLocation.Name,
                Capacity = existingTableLocation.Capacity,
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

            // Check if table location has active bookings
            var bookings = await _bookingRepository.GetAllAsync();
            var hasActiveBookings = bookings.Any(b => 
                b.TableLocationId == id && 
                b.ReservationDate >= DateTime.Now &&
                b.Status != "Cancelled");
            
            if (hasActiveBookings)
            {
                return Result.Failure("Cannot delete table location with active bookings. Please cancel or complete bookings first.", 400);
            }

            await _tableLocationRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting table location: {ex.Message}", 500);
        }
    }

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

    public async Task<Result<IEnumerable<TableLocationDto>>> GetAvailableTablesAsync(DateTime reservationDate, int numberOfGuests)
    {
        try
        {
            var tableLocations = await _tableLocationRepository.GetAllAsync();
            var bookings = await _bookingRepository.GetAllAsync();

            var availableTables = tableLocations.Where(tl => 
                tl.Capacity >= numberOfGuests &&
                !bookings.Any(b => 
                    b.TableLocationId == tl.Id &&
                    b.ReservationDate.Date == reservationDate.Date &&
                    Math.Abs((b.ReservationDate - reservationDate).TotalHours) < 2 &&
                    b.Status != "Cancelled")
            );

            var tableLocationDtos = availableTables.Select(tl => new TableLocationDto
            {
                Id = tl.Id,
                Name = tl.Name,
                Capacity = tl.Capacity,
                IsAvailable = true
            }).ToList();

            return Result<IEnumerable<TableLocationDto>>.Success(tableLocationDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TableLocationDto>>.Failure($"Error retrieving available tables: {ex.Message}", 500);
        }
    }
} 