using TableReservations.Common.Results;
using TableReservations.DTOs;
using TableReservations.Models;
using TableReservations.Repositories;
using TableReservations.Services.Interfaces;

namespace TableReservations.Services;

/// <summary>
/// Booking service implementation containing business logic for table reservations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITableLocationRepository _tableLocationRepository;

    public BookingService(IBookingRepository bookingRepository, ITableLocationRepository tableLocationRepository)
    {
        _bookingRepository = bookingRepository;
        _tableLocationRepository = tableLocationRepository;
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync()
    {
        try
        {
            var bookings = await _bookingRepository.GetAllAsync();
            
            var bookingDtos = bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                FullName = b.FullName,
                Email = b.Email,
                PhoneNumber = b.PhoneNumber,
                DateTime = b.DateTime,
                NumberOfGuests = b.NumberOfGuests,
                TableLocation = b.TableLocation,
                SpecialRequest = b.SpecialRequest,
                CreatedAt = DateTime.Now // Model doesn't have CreatedAt, set current time
            }).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings: {ex.Message}", 500);
        }
    }

    // GetAllBookingsAsync with pagination
    public async Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync(int page, int pageSize)
    {
        try
        {
            if (page <= 0 || pageSize <= 0)
            {
                return Result<IEnumerable<BookingDto>>.Failure("Page and pageSize must be positive integers", 400);
            }

            var allBookings = await _bookingRepository.GetAllAsync();
            var paginatedBookings = allBookings
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            
            var bookingDtos = paginatedBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                FullName = b.FullName,
                Email = b.Email,
                PhoneNumber = b.PhoneNumber,
                DateTime = b.DateTime,
                NumberOfGuests = b.NumberOfGuests,
                TableLocation = b.TableLocation,
                SpecialRequest = b.SpecialRequest,
                CreatedAt = DateTime.Now
            }).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings: {ex.Message}", 500);
        }
    }

    public async Task<Result<BookingDto>> GetBookingByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result<BookingDto>.Failure("Invalid booking ID", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(id);
            
            if (booking == null)
            {
                return Result<BookingDto>.NotFound($"Booking with ID {id} not found");
            }

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                FullName = booking.FullName,
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                DateTime = booking.DateTime,
                NumberOfGuests = booking.NumberOfGuests,
                TableLocation = booking.TableLocation,
                SpecialRequest = booking.SpecialRequest,
                CreatedAt = DateTime.Now
            };

            return Result<BookingDto>.Success(bookingDto);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error retrieving booking: {ex.Message}", 500);
        }
    }

    public async Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto)
    {
        try
        {
            // Validate reservation date
            if (bookingDto.DateTime <= DateTime.Now)
            {
                return Result<BookingDto>.Failure("Reservation date must be in the future", 400);
            }

            // Check table availability for the specific table location string
            var existingBookings = await _bookingRepository.GetAllAsync();
            var isTableAvailable = !existingBookings.Any(b => 
                b.TableLocation.Equals(bookingDto.TableLocation, StringComparison.OrdinalIgnoreCase) &&
                b.DateTime.Date == bookingDto.DateTime.Date &&
                Math.Abs((b.DateTime - bookingDto.DateTime).TotalHours) < 2);

            if (!isTableAvailable)
            {
                return Result<BookingDto>.Failure("Table is not available at the selected time", 400);
            }

            var booking = new Booking
            {
                FullName = bookingDto.FullName,
                Email = bookingDto.Email,
                PhoneNumber = bookingDto.PhoneNumber,
                DateTime = bookingDto.DateTime,
                NumberOfGuests = bookingDto.NumberOfGuests,
                TableLocation = bookingDto.TableLocation,
                SpecialRequest = bookingDto.SpecialRequest
            };

            await _bookingRepository.AddAsync(booking);

            var createdBookingDto = new BookingDto
            {
                Id = booking.Id,
                FullName = booking.FullName,
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                DateTime = booking.DateTime,
                NumberOfGuests = booking.NumberOfGuests,
                TableLocation = booking.TableLocation,
                SpecialRequest = booking.SpecialRequest,
                CreatedAt = DateTime.Now
            };

            return Result<BookingDto>.Success(createdBookingDto, 201);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error creating booking: {ex.Message}", 500);
        }
    }

    // UpdateBookingAsync
    public async Task<Result<BookingDto>> UpdateBookingAsync(int id, CreateBookingDto bookingDto)
    {
        try
        {
            if (id <= 0)
            {
                return Result<BookingDto>.Failure("Invalid booking ID", 400);
            }

            var existingBooking = await _bookingRepository.GetByIdAsync(id);
            if (existingBooking == null)
            {
                return Result<BookingDto>.NotFound($"Booking with ID {id} not found");
            }

            // Validate reservation date
            if (bookingDto.DateTime <= DateTime.Now)
            {
                return Result<BookingDto>.Failure("Reservation date must be in the future", 400);
            }

            // Update booking properties
            existingBooking.FullName = bookingDto.FullName;
            existingBooking.Email = bookingDto.Email;
            existingBooking.PhoneNumber = bookingDto.PhoneNumber;
            existingBooking.DateTime = bookingDto.DateTime;
            existingBooking.NumberOfGuests = bookingDto.NumberOfGuests;
            existingBooking.TableLocation = bookingDto.TableLocation;
            existingBooking.SpecialRequest = bookingDto.SpecialRequest;

            await _bookingRepository.UpdateAsync(existingBooking);

            var updatedBookingDto = new BookingDto
            {
                Id = existingBooking.Id,
                FullName = existingBooking.FullName,
                Email = existingBooking.Email,
                PhoneNumber = existingBooking.PhoneNumber,
                DateTime = existingBooking.DateTime,
                NumberOfGuests = existingBooking.NumberOfGuests,
                TableLocation = existingBooking.TableLocation,
                SpecialRequest = existingBooking.SpecialRequest,
                CreatedAt = DateTime.Now
            };

            return Result<BookingDto>.Success(updatedBookingDto);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error updating booking: {ex.Message}", 500);
        }
    }

    public async Task<Result<BookingDto>> UpdateBookingStatusAsync(int id, string status)
    {
        try
        {
            // Note: Booking model doesn't have Status field, this method may need to be removed
            // or we need to add Status to the Booking model
            return Result<BookingDto>.Failure("Booking status update not supported - no Status field in model", 400);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error updating booking status: {ex.Message}", 500);
        }
    }

    // CancelBookingAsync - since no Status field, we'll delete the booking
    public async Task<Result> CancelBookingAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid booking ID", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return Result.NotFound($"Booking with ID {id} not found");
            }

            // Since there's no Status field, we'll delete the booking to "cancel" it
            await _bookingRepository.DeleteAsync(id);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error cancelling booking: {ex.Message}", 500);
        }
    }

    public async Task<Result> DeleteBookingAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                return Result.Failure("Invalid booking ID", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return Result.NotFound($"Booking with ID {id} not found");
            }

            await _bookingRepository.DeleteAsync(id);
            return Result.Success(204);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting booking: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetBookingsByDateAsync(DateTime date)
    {
        try
        {
            var bookings = await _bookingRepository.GetAllAsync();
            var filteredBookings = bookings.Where(b => b.DateTime.Date == date.Date);
            
            var bookingDtos = filteredBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                FullName = b.FullName,
                Email = b.Email,
                PhoneNumber = b.PhoneNumber,
                DateTime = b.DateTime,
                NumberOfGuests = b.NumberOfGuests,
                TableLocation = b.TableLocation,
                SpecialRequest = b.SpecialRequest,
                CreatedAt = DateTime.Now
            }).OrderBy(b => b.DateTime).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings by date: {ex.Message}", 500);
        }
    }

    // GetBookingsByDateRangeAsync
    public async Task<Result<IEnumerable<BookingDto>>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            if (startDate > endDate)
            {
                return Result<IEnumerable<BookingDto>>.Failure("Start date cannot be later than end date", 400);
            }

            var bookings = await _bookingRepository.GetAllAsync();
            var filteredBookings = bookings.Where(b => 
                b.DateTime.Date >= startDate.Date && 
                b.DateTime.Date <= endDate.Date);
            
            var bookingDtos = filteredBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                FullName = b.FullName,
                Email = b.Email,
                PhoneNumber = b.PhoneNumber,
                DateTime = b.DateTime,
                NumberOfGuests = b.NumberOfGuests,
                TableLocation = b.TableLocation,
                SpecialRequest = b.SpecialRequest,
                CreatedAt = DateTime.Now
            }).OrderBy(b => b.DateTime).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings by date range: {ex.Message}", 500);
        }
    }

    public async Task<Result<bool>> IsTableAvailableAsync(int tableLocationId, DateTime reservationDate)
    {
        try
        {
            // Note: This method signature doesn't match our string-based TableLocation
            // We should probably deprecate this in favor of string-based method
            return Result<bool>.Failure("Method deprecated - use CheckTableAvailabilityAsync with string tableLocation", 400);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking table availability: {ex.Message}", 500);
        }
    }

    // CheckTableAvailabilityAsync
    public async Task<Result<bool>> CheckTableAvailabilityAsync(string tableLocationName, DateTime reservationDate, int duration)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tableLocationName))
            {
                return Result<bool>.Failure("Table location name cannot be empty", 400);
            }

            if (reservationDate <= DateTime.Now)
            {
                return Result<bool>.Success(false); // Past dates are not available
            }

            var existingBookings = await _bookingRepository.GetAllAsync();
            var endTime = reservationDate.AddHours(duration);
            
            var isAvailable = !existingBookings.Any(b => 
                b.TableLocation.Equals(tableLocationName, StringComparison.OrdinalIgnoreCase) &&
                !(b.DateTime.AddHours(2) <= reservationDate || // Booking ends before our start
                  b.DateTime >= endTime)); // Booking starts after our end

            return Result<bool>.Success(isAvailable);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking table availability: {ex.Message}", 500);
        }
    }

    public async Task<Result<IEnumerable<BookingDto>>> GetBookingsByStatusAsync(string status)
    {
        try
        {
            // Note: Booking model doesn't have Status field
            // This method might not be applicable or we need to add Status to model
            return Result<IEnumerable<BookingDto>>.Failure("Booking status filtering not supported - no Status field in model", 400);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings by status: {ex.Message}", 500);
        }
    }

    // GetBookingStatisticsAsync
    public async Task<Result<object>> GetBookingStatisticsAsync()
    {
        try
        {
            var bookings = await _bookingRepository.GetAllAsync();
            
            var statistics = new
            {
                TotalBookings = bookings.Count(),
                TodayBookings = bookings.Count(b => b.DateTime.Date == DateTime.Today),
                ThisWeekBookings = bookings.Count(b => b.DateTime.Date >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) && 
                                                      b.DateTime.Date < DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek)),
                ThisMonthBookings = bookings.Count(b => b.DateTime.Month == DateTime.Now.Month && 
                                                        b.DateTime.Year == DateTime.Now.Year),
                AverageGuestsPerBooking = bookings.Any() ? bookings.Average(b => b.NumberOfGuests) : 0,
                PopularTableLocations = bookings.GroupBy(b => b.TableLocation)
                                               .Select(g => new { TableLocation = g.Key, Count = g.Count() })
                                               .OrderByDescending(x => x.Count)
                                               .Take(5)
                                               .ToList()
            };

            return Result<object>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Error retrieving booking statistics: {ex.Message}", 500);
        }
    }
} 