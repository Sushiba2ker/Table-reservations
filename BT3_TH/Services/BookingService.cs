using BT3_TH.Common.Results;
using BT3_TH.DTOs;
using BT3_TH.Models;
using BT3_TH.Repositories;
using BT3_TH.Services.Interfaces;

namespace BT3_TH.Services;

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
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                TableLocationId = b.TableLocationId,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings: {ex.Message}", 500);
        }
    }

    // NEW METHOD: GetAllBookingsAsync with pagination
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
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                TableLocationId = b.TableLocationId,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt
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
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                TableLocationId = booking.TableLocationId,
                ReservationDate = booking.ReservationDate,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
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
            if (bookingDto.ReservationDate <= DateTime.Now)
            {
                return Result<BookingDto>.Failure("Reservation date must be in the future", 400);
            }

            // Check if table location exists
            var tableLocation = await _tableLocationRepository.GetByIdAsync(bookingDto.TableLocationId);
            if (tableLocation == null)
            {
                return Result<BookingDto>.Failure($"Table location with ID {bookingDto.TableLocationId} not found", 400);
            }

            // Check table availability
            var existingBookings = await _bookingRepository.GetAllAsync();
            var isTableAvailable = !existingBookings.Any(b => 
                b.TableLocationId == bookingDto.TableLocationId &&
                b.ReservationDate.Date == bookingDto.ReservationDate.Date &&
                Math.Abs((b.ReservationDate - bookingDto.ReservationDate).TotalHours) < 2 &&
                b.Status != "Cancelled");

            if (!isTableAvailable)
            {
                return Result<BookingDto>.Failure("Table is not available at the selected time", 400);
            }

            var booking = new Booking
            {
                CustomerName = bookingDto.CustomerName,
                CustomerEmail = bookingDto.CustomerEmail,
                CustomerPhone = bookingDto.CustomerPhone,
                TableLocationId = bookingDto.TableLocationId,
                ReservationDate = bookingDto.ReservationDate,
                NumberOfGuests = bookingDto.NumberOfGuests,
                SpecialRequests = bookingDto.SpecialRequests,
                Status = "Confirmed",
                CreatedAt = DateTime.Now
            };

            await _bookingRepository.AddAsync(booking);

            var createdBookingDto = new BookingDto
            {
                Id = booking.Id,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                TableLocationId = booking.TableLocationId,
                ReservationDate = booking.ReservationDate,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };

            return Result<BookingDto>.Success(createdBookingDto, 201);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error creating booking: {ex.Message}", 500);
        }
    }

    // NEW METHOD: UpdateBookingAsync
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
            if (bookingDto.ReservationDate <= DateTime.Now)
            {
                return Result<BookingDto>.Failure("Reservation date must be in the future", 400);
            }

            // Check if table location exists
            var tableLocation = await _tableLocationRepository.GetByIdAsync(bookingDto.TableLocationId);
            if (tableLocation == null)
            {
                return Result<BookingDto>.Failure($"Table location with ID {bookingDto.TableLocationId} not found", 400);
            }

            // Update booking properties
            existingBooking.CustomerName = bookingDto.CustomerName;
            existingBooking.CustomerEmail = bookingDto.CustomerEmail;
            existingBooking.CustomerPhone = bookingDto.CustomerPhone;
            existingBooking.TableLocationId = bookingDto.TableLocationId;
            existingBooking.ReservationDate = bookingDto.ReservationDate;
            existingBooking.NumberOfGuests = bookingDto.NumberOfGuests;
            existingBooking.SpecialRequests = bookingDto.SpecialRequests;

            await _bookingRepository.UpdateAsync(existingBooking);

            var updatedBookingDto = new BookingDto
            {
                Id = existingBooking.Id,
                CustomerName = existingBooking.CustomerName,
                CustomerEmail = existingBooking.CustomerEmail,
                CustomerPhone = existingBooking.CustomerPhone,
                TableLocationId = existingBooking.TableLocationId,
                ReservationDate = existingBooking.ReservationDate,
                NumberOfGuests = existingBooking.NumberOfGuests,
                SpecialRequests = existingBooking.SpecialRequests,
                Status = existingBooking.Status,
                CreatedAt = existingBooking.CreatedAt
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
            if (id <= 0)
            {
                return Result<BookingDto>.Failure("Invalid booking ID", 400);
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                return Result<BookingDto>.Failure("Status cannot be empty", 400);
            }

            var validStatuses = new[] { "Confirmed", "Completed", "Cancelled", "No-Show" };
            if (!validStatuses.Contains(status))
            {
                return Result<BookingDto>.Failure($"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}", 400);
            }

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return Result<BookingDto>.NotFound($"Booking with ID {id} not found");
            }

            booking.Status = status;
            await _bookingRepository.UpdateAsync(booking);

            var updatedBookingDto = new BookingDto
            {
                Id = booking.Id,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                TableLocationId = booking.TableLocationId,
                ReservationDate = booking.ReservationDate,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status,
                CreatedAt = booking.CreatedAt
            };

            return Result<BookingDto>.Success(updatedBookingDto);
        }
        catch (Exception ex)
        {
            return Result<BookingDto>.Failure($"Error updating booking status: {ex.Message}", 500);
        }
    }

    // NEW METHOD: CancelBookingAsync
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

            booking.Status = "Cancelled";
            await _bookingRepository.UpdateAsync(booking);
            
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
            var filteredBookings = bookings.Where(b => b.ReservationDate.Date == date.Date);
            
            var bookingDtos = filteredBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                TableLocationId = b.TableLocationId,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }).OrderBy(b => b.ReservationDate).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings by date: {ex.Message}", 500);
        }
    }

    // NEW METHOD: GetBookingsByDateRangeAsync
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
                b.ReservationDate.Date >= startDate.Date && 
                b.ReservationDate.Date <= endDate.Date);
            
            var bookingDtos = filteredBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                TableLocationId = b.TableLocationId,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }).OrderBy(b => b.ReservationDate).ToList();

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
            if (tableLocationId <= 0)
            {
                return Result<bool>.Failure("Invalid table location ID", 400);
            }

            if (reservationDate <= DateTime.Now)
            {
                return Result<bool>.Success(false); // Past dates are not available
            }

            // Check if table location exists
            var tableLocation = await _tableLocationRepository.GetByIdAsync(tableLocationId);
            if (tableLocation == null)
            {
                return Result<bool>.Success(false); // Non-existent table is not available
            }

            var existingBookings = await _bookingRepository.GetAllAsync();
            var isAvailable = !existingBookings.Any(b => 
                b.TableLocationId == tableLocationId &&
                b.ReservationDate.Date == reservationDate.Date &&
                Math.Abs((b.ReservationDate - reservationDate).TotalHours) < 2 &&
                b.Status != "Cancelled");

            return Result<bool>.Success(isAvailable);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error checking table availability: {ex.Message}", 500);
        }
    }

    // NEW METHOD: CheckTableAvailabilityAsync
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

            // Find table by name
            var allTables = await _tableLocationRepository.GetAllAsync();
            var table = allTables.FirstOrDefault(t => t.TableName.Equals(tableLocationName, StringComparison.OrdinalIgnoreCase));
            
            if (table == null)
            {
                return Result<bool>.Success(false); // Non-existent table is not available
            }

            var existingBookings = await _bookingRepository.GetAllAsync();
            var endTime = reservationDate.AddHours(duration);
            
            var isAvailable = !existingBookings.Any(b => 
                b.TableLocationId == table.Id &&
                b.Status != "Cancelled" &&
                !(b.ReservationDate.AddHours(2) <= reservationDate || // Booking ends before our start
                  b.ReservationDate >= endTime)); // Booking starts after our end

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
            if (string.IsNullOrWhiteSpace(status))
            {
                return Result<IEnumerable<BookingDto>>.Failure("Status cannot be empty", 400);
            }

            var bookings = await _bookingRepository.GetAllAsync();
            var filteredBookings = bookings.Where(b => b.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            
            var bookingDtos = filteredBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                CustomerName = b.CustomerName,
                CustomerEmail = b.CustomerEmail,
                CustomerPhone = b.CustomerPhone,
                TableLocationId = b.TableLocationId,
                ReservationDate = b.ReservationDate,
                NumberOfGuests = b.NumberOfGuests,
                SpecialRequests = b.SpecialRequests,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            }).OrderBy(b => b.ReservationDate).ToList();

            return Result<IEnumerable<BookingDto>>.Success(bookingDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<BookingDto>>.Failure($"Error retrieving bookings by status: {ex.Message}", 500);
        }
    }

    // NEW METHOD: GetBookingStatisticsAsync
    public async Task<Result<object>> GetBookingStatisticsAsync()
    {
        try
        {
            var bookings = await _bookingRepository.GetAllAsync();
            
            var statistics = new
            {
                TotalBookings = bookings.Count(),
                ConfirmedBookings = bookings.Count(b => b.Status == "Confirmed"),
                CompletedBookings = bookings.Count(b => b.Status == "Completed"),
                CancelledBookings = bookings.Count(b => b.Status == "Cancelled"),
                NoShowBookings = bookings.Count(b => b.Status == "No-Show"),
                TodayBookings = bookings.Count(b => b.ReservationDate.Date == DateTime.Today),
                ThisWeekBookings = bookings.Count(b => b.ReservationDate.Date >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek) && 
                                                      b.ReservationDate.Date < DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek)),
                ThisMonthBookings = bookings.Count(b => b.ReservationDate.Month == DateTime.Now.Month && 
                                                        b.ReservationDate.Year == DateTime.Now.Year),
                AverageGuestsPerBooking = bookings.Any() ? bookings.Average(b => b.NumberOfGuests) : 0
            };

            return Result<object>.Success(statistics);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure($"Error retrieving booking statistics: {ex.Message}", 500);
        }
    }
} 