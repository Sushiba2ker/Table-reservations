using System.ComponentModel.DataAnnotations;

namespace BT3_TH.Models
{
    public class TableLocation
    {
        public int Id { get; set; }
        
        [Required]
        public required string Name { get; set; }
        
        public string? ImageUrl { get; set; }
    }
}