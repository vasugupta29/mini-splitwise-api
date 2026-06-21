using System.ComponentModel.DataAnnotations;

namespace MiniSplitwise.Api.DTOs
{
    public class GroupUpdateDto
    {
        [Required, MaxLength(100)] 
        public string? Name { get; set; } =  null!;

        [MaxLength(500)]
        public string? Description { get; set; } 
    }
}
