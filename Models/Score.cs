using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace score_api.Models
{
    public class Score
    {
        public int Id { get; set; }
        
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required, MinLength(3), MaxLength(500)]
        public string Module { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(5,2)")]
        public double Mark { get; set; } = 0;
        
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
    }
}