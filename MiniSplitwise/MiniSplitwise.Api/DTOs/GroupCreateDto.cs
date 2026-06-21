using System.ComponentModel.DataAnnotations;

namespace MiniSplitwise.Api.DTOs
{
    public class GroupCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        // The creator (CreatedByUserId) is auto-added as an Admin member by the service.
        // This list is the OTHER members to add at creation — do not include the creator here.
        public List<int> MemberUserIds { get; set; } = new();

        // Temporary: will come from the JWT token once auth is added (Day 28), then removed from this DTO.
        public int CreatedByUserId { get; set; }
    }
}
