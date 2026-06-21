namespace MiniSplitwise.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation: a User has many GroupMember rows
        public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    }
}
