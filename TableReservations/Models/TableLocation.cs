using System.ComponentModel.DataAnnotations;

namespace TableReservations.Models
{
    public class TableLocation
    {
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        public string? ImageUrl { get; set; }
    }
}