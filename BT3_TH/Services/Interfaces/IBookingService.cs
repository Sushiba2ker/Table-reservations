using BT3_TH.Common.Results;
using BT3_TH.DTOs;

namespace BT3_TH.Services.Interfaces;

/// <summary>
/// Service interface for Booking business logic
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Create new booking
    /// </summary>
    Task<Result<BookingDto>> CreateBookingAsync(CreateBookingDto bookingDto);
    
    /// <summary>
    /// Get booking by ID
    /// </summary>
    Task<Result<BookingDto>> GetBookingByIdAsync(int id);
    
    /// <summary>
    /// Get all bookings with pagination
    /// </summary>
    Task<Result<IEnumerable<BookingDto>>> GetAllBookingsAsync(int page = 1, int pageSize = 10);
    
    /// <summary>
    /// Get bookings by date range
    /// </summary>
    Task<Result<IEnumerable<BookingDto>>> GetBookingsByDateRangeAsync(DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Update booking
    /// </summary>
    Task<Result<BookingDto>> UpdateBookingAsync(int id, CreateBookingDto bookingDto);
    
    /// <summary>
    /// Cancel booking
    /// </summary>
    Task<Result> CancelBookingAsync(int id);
    
    /// <summary>
    /// Check table availability
    /// </summary>
    Task<Result<bool>> CheckTableAvailabilityAsync(string tableLocation, DateTime dateTime, int duration = 2);
    
    /// <summary>
    /// Get booking statistics
    /// </summary>
    Task<Result<object>> GetBookingStatisticsAsync();
} 