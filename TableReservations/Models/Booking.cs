using System;
using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public required string FullName { get; set; }

        // Số điện thoại
        [Required, StringLength(10)]
        public required string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        public required string TableLocation { get; set; }

        public string? SpecialRequest { get; set; }
    }
}