using MiniSplitwise.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace MiniSplitwise.Api.DTOs
{
    public class AddMemberDto
    {
        [Required]
        public int UserId { get; set; }

        public GroupRole Role { get; set; } = GroupRole.Member; // Default role is "Member"
    }
}
