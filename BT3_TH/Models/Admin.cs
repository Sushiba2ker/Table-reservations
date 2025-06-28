using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models;

public class Admin
{
    [Required]
    public required string Username { get; set; }
    
    [Required]
    public required string Password { get; set; }
}