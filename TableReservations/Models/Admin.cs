using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models;

public class Admin
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}