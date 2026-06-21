using MiniSplitwise.Api.Models;

namespace MiniSplitwise.Api.DTOs
{
    public class GroupMemberDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; } 
    }
}
