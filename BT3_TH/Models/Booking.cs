using System;
using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string FullName { get; set; }

        // Số điện thoại
        [Required, StringLength(10)]
        public string PhoneNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        public string TableLocation { get; set; }

        public string SpecialRequest { get; set; }
    }
}