﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models
{
    public class ApplicationUser : IdentityUser 
    {
        [Required]
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }
    }

}
