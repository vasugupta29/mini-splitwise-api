namespace MiniSplitwise.Api.Models;

public class GroupMember
{
    public int Id { get; set; }

    // Foreign keys
    public int UserId { get; set; }
    public int GroupId { get; set; }

    // Relationship-specific data
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public GroupRole Role { get; set; } = GroupRole.Member;

    // Navigation properties (EF Core fills these in)
    public User User { get; set; } = null!;
    public Group Group { get; set; } = null!;
}

public enum GroupRole
{
    Member = 0,
    Admin = 1
}