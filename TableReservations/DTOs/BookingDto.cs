using System.ComponentModel.DataAnnotations;

namespace TableReservations.DTOs;

/// <summary>
/// Data Transfer Object for Booking information
/// </summary>
public class BookingDto
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string FullName { get; set; }
    
    [Required]
    [StringLength(10)]
    [Phone]
    public required string PhoneNumber { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public DateTime DateTime { get; set; }
    
    [Required]
    [Range(1, 20)]
    public int NumberOfGuests { get; set; }
    
    [Required]
    public required string TableLocation { get; set; }
    
    public string? SpecialRequest { get; set; }
    
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for creating bookings
/// </summary>
public class CreateBookingDto
{
    [Required]
    [StringLength(50)]
    public required string FullName { get; set; }
    
    [Required]
    [StringLength(10)]
    [Phone]
    public required string PhoneNumber { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    
    [Required]
    public DateTime DateTime { get; set; }
    
    [Required]
    [Range(1, 20)]
    public int NumberOfGuests { get; set; }
    
    [Required]
    public required string TableLocation { get; set; }
    
    public string? SpecialRequest { get; set; }
} 