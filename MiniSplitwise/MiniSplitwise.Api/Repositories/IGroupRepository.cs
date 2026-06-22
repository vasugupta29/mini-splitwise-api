using Microsoft.EntityFrameworkCore.Storage;
using MiniSplitwise.Api.Models;

namespace MiniSplitwise.Api.Repositories
{
    public interface IGroupRepository
    {
        // --- Group itself ---

        // Fetch a group WITH its members (and each member's User), or null if not found.
        Task<Group?> GetByIdAsync(int id, CancellationToken ct = default);

        // Fetch a group WITHOUT loading members — for update/existence checks where
        // you don't need the member graph.
        Task<Group?> GetByIdBasicAsync(int id, CancellationToken ct = default);

        void Add(Group group);

        // --- Membership ---

        // Has this user already got a membership row in this group?
        // (check-then-act guard; the unique index is the race backstop)
        Task<bool> IsMemberAsync(int groupId, int userId, CancellationToken ct = default);

        void AddMember(GroupMember member);

        // Fetch a single membership row (e.g. to remove it), or null.
        Task<GroupMember?> GetMemberAsync(int groupId, int userId, CancellationToken ct = default);

        void RemoveMember(GroupMember member);   // EF tracks the delete; SaveChanges commits it

        // List the members of a group (with their User), for GET /groups/{id}/members.
        Task<List<GroupMember>> GetMembersAsync(int groupId, CancellationToken ct = default);
        Task<List<Group>> GetGroupsForUserAsync(int userId, CancellationToken ct = default);

        // --- Persistence + transactions ---

        Task SaveChangesAsync(CancellationToken ct = default);

        // For the create flow: group + member rows must commit atomically.
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default);
    }
}