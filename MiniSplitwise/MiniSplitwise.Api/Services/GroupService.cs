using MiniSplitwise.Api.DTOs;
using MiniSplitwise.Api.Models;
using MiniSplitwise.Api.Repositories;
using MiniSplitwise.Api.Exceptions;

namespace MiniSplitwise.Api.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GroupService> _logger;
        
        public GroupService(IGroupRepository groupRepository, IUserRepository userRepository, ILogger<GroupService> logger)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GroupResponseDto> CreateAsync(GroupCreateDto groupCreateDto, CancellationToken ct = default)
        {
            var name = groupCreateDto.Name.Trim();

            // 1. Build the set of all ids to validate: creator + members. Validate in ONE call.
            var memberIds = new HashSet<int>(groupCreateDto.MemberUserIds);
            var idsToValidate = new HashSet<int>(memberIds) { groupCreateDto.CreatedByUserId };
            var existingIds = await _userRepository.GetExistingUserIdsAsync(idsToValidate, ct);
            var existingSet = existingIds.ToHashSet();

            // 2. Creator not in existing set -> BadRequestException.
            if(!existingSet.Contains(groupCreateDto.CreatedByUserId))
                throw new BadRequestException($"Creator user id {groupCreateDto.CreatedByUserId} does not exist.");

            // 3. Dedup members (HashSet), exclude the creator's id.
            memberIds.Remove(groupCreateDto.CreatedByUserId);

            // 4. Any remaining member id not in existing set -> BadRequestException (say which).
            var missingMembers = memberIds.Where(id => !existingSet.Contains(id)).ToList();
            if (missingMembers.Any())
                throw new BadRequestException($"The following member user ids do not exist: {string.Join(", ", missingMembers)}");

            // 5. Build Group; add creator as Admin GroupMember; add each member as Member.
            var group = new Group
            {
                Name = name,
                Description = groupCreateDto.Description?.Trim(),
                CreatedByUserId = groupCreateDto.CreatedByUserId,
                Members = new List<GroupMember>
                {
                    new GroupMember
                    {
                        UserId = groupCreateDto.CreatedByUserId,
                        Role = GroupRole.Admin
                    }
                }
            };

            foreach (var memberId in memberIds)
            {
                group.Members.Add(new GroupMember
                {
                    UserId = memberId,
                    Role = GroupRole.Member
                });
            }

            // 6. _groupRepository.Add(group); await SaveChangesAsync(ct);
            _groupRepository.Add(group);
            await _groupRepository.SaveChangesAsync(ct);

            // 7. Re-fetch via GetByIdAsync(group.Id, ct); map to GroupResponseDto; return.
            var created = await _groupRepository.GetByIdAsync(group.Id, ct);
            return MapToDto(created!);
        }

        public async Task<GroupResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var group = await _groupRepository.GetByIdAsync(id, ct);
            return group is not null ? MapToDto(group) : null;
        }

        public async Task<GroupResponseDto?> UpdateAsync(int id, GroupUpdateDto dto, CancellationToken ct = default)
        {
            var group = await _groupRepository.GetByIdAsync(id, ct);
            if (group is null)
                return null;
            group.Name = dto.Name?.Trim() ?? group.Name;
            group.Description = dto.Description?.Trim() ?? group.Description;
            await _groupRepository.SaveChangesAsync(ct);
            return MapToDto(group);
        }

        public async Task<List<GroupMemberDto>?> GetMembersAsync(int groupId, CancellationToken ct = default)
        {
            var members = await _groupRepository.GetMembersAsync(groupId, ct);
            return members.Select(m => new GroupMemberDto
            {
                UserId = m.UserId,
                Name = m.User.Name,
                Role = m.Role,
                JoinedAt = m.JoinedAt
            }).ToList();
        }

        public async Task<GroupMemberDto> AddMemberAsync(int groupId, AddMemberDto dto, CancellationToken ct = default)
        {
            var group = await _groupRepository.GetByIdBasicAsync(groupId, ct);
            if (group is null)
                throw new NotFoundException($"Group with id {groupId} not found.");
            if (!await _userRepository.ExistsAsync(dto.UserId, ct))
                throw new BadRequestException($"User with id {dto.UserId} does not exist.");
            if (await _groupRepository.IsMemberAsync(groupId, dto.UserId, ct))
                throw new BadRequestException($"User with id {dto.UserId} is already a member of group {groupId}.");
            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = dto.UserId,
                Role = GroupRole.Member
            };
            _groupRepository.AddMember(member);
            await _groupRepository.SaveChangesAsync(ct);
            // Fetch the member with User for the response DTO
            var addedMember = await _groupRepository.GetMemberAsync(groupId, dto.UserId, ct);
            if (addedMember is null)
                throw new InvalidOperationException("Failed to retrieve the newly added member.");
            return new GroupMemberDto
            {
                UserId = addedMember.UserId,
                Name = addedMember.User.Name,
                Role = addedMember.Role,
                JoinedAt = addedMember.JoinedAt
            };
        }

        public async Task<bool> RemoveMemberAsync(int groupId, int userId, CancellationToken ct = default)
        {
            var group = await _groupRepository.GetByIdAsync(groupId, ct);
            if (group is null)
                return false;
            var member = group.Members.FirstOrDefault(m => m.UserId == userId);
            if (member is null)
                return false;
            group.Members.Remove(member);
            await _groupRepository.SaveChangesAsync(ct);
            return true;
        }

        private static GroupResponseDto MapToDto(Group group)
        {
            return new GroupResponseDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedByUserId = group.CreatedByUserId,
                CreatedAt = group.CreatedAt,
                Members = group.Members.Select(m => new GroupMemberDto
                {
                    UserId = m.UserId,
                    Name = m.User.Name,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
                }).ToList()
            };
        }
    }
}
