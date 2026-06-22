using MiniSplitwise.Api.DTOs;

namespace MiniSplitwise.Api.Services
{
    public interface IGroupService
    {
        Task<GroupResponseDto> CreateAsync(GroupCreateDto dto, CancellationToken ct = default);
        Task<GroupResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<GroupResponseDto?> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default);
        Task<List<GroupMemberDto>?> GetMembersAsync(int groupId, CancellationToken ct = default);
        Task<GroupMemberDto> AddMemberAsync(int groupId, AddMemberDto dto, CancellationToken ct = default);
        Task<bool> RemoveMemberAsync(int groupId, int userId, CancellationToken ct = default);
        Task<List<GroupSummaryDto>?> GetGroupsForUserAsync(int  userId, CancellationToken ct = default);
    }
}