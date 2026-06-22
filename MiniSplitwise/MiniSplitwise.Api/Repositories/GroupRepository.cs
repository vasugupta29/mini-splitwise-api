using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MiniSplitwise.Api.Data;
using MiniSplitwise.Api.Models;

namespace MiniSplitwise.Api.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly AppDbContext _context;
        public GroupRepository(AppDbContext context)
        {
            _context = context;
        }
        // Implement the methods defined in IGroupRepository here
        // For example:
        public async Task<Group?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Groups
                .AsNoTracking()
                .Include(g => g.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(g => g.Id == id, ct);
        }
        public async Task<Group?> GetByIdBasicAsync(int id, CancellationToken ct = default)
        {
            return await _context.Groups.FirstOrDefaultAsync(g => g.Id == id, ct);
        }

        public void Add(Group group)
        {
            _context.Groups.Add(group);
        }

        public async Task<bool> IsMemberAsync(int groupId, int userId, CancellationToken ct = default)
        {
            return await _context.GroupMembers.AnyAsync(m => m.GroupId == groupId && m.UserId == userId, ct);
        }

        public void AddMember(GroupMember member)
        {
            _context.GroupMembers.Add(member);
        }

        public async Task<GroupMember?> GetMemberAsync(int groupId, int userId, CancellationToken ct = default)
        {
            return await _context.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId, ct);
        }
        public void RemoveMember(GroupMember member)
        {
            _context.GroupMembers.Remove(member);
        }

        public async Task<List<GroupMember>> GetMembersAsync(int groupId, CancellationToken ct = default)
        {
            return await _context.GroupMembers
                .AsNoTracking()
                .Where(m => m.GroupId == groupId)
                .Include(m => m.User)
                .OrderBy(m => m.JoinedAt)
                .ToListAsync(ct);
        }

        public async Task<List<Group>> GetGroupsForUserAsync(int userId, CancellationToken ct = default)
        {
            return await _context.Groups
                .AsNoTracking()
                .Where(g => g.Members.Any(m => m.UserId == userId))
                .ToListAsync(ct);
        }
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken ct = default)
        {
            return await _context.Database.BeginTransactionAsync(ct);
        }
    }
}
