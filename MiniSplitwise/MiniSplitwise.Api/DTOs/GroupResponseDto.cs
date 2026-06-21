namespace MiniSplitwise.Api.DTOs
{
    public class GroupResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GroupMemberDto> Members { get; set; } = new List<GroupMemberDto>();
    }
}
