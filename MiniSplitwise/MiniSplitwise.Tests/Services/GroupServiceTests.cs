using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using MiniSplitwise.Api.DTOs;
using MiniSplitwise.Api.Models;
using MiniSplitwise.Api.Repositories;
using MiniSplitwise.Api.Services;
using Moq;

namespace MiniSplitwise.Tests.Services;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groupRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly GroupService _service;

    public GroupServiceTests()
    {
        _groupRepoMock = new Mock<IGroupRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _service = new GroupService(_groupRepoMock.Object, _userRepoMock.Object, NullLogger<GroupService>.Instance);
    }

    [Fact]
    public async Task Create_WithValidCreatorAndMembers_ReturnsGroupWithMenbers()
    {
        //Arrange
        var dto = new GroupCreateDto
        {
            Name = "Test Group",
            Description = "A test group",
            CreatedByUserId = 1,
            MemberUserIds = new List<int> { 2 }
        };

        // Validation passes: creator (1) and member (2) both "exist"
        _userRepoMock.Setup(r => r.GetExistingUserIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new List<int> { 1, 2 });

        // The re-fetch after save MUST return a populated graph — the service maps THIS,
        // not the object it built. If you skip this setup it returns null and MapToDto NREs.
        var createdGroup = new Group
        {
            Id = 1,
            Name = dto.Name,
            Description = dto.Description,
            CreatedByUserId = dto.CreatedByUserId,
            Members = new List<GroupMember>
            {
                new GroupMember { UserId = 1, Role = GroupRole.Admin, User = new User { Id = 1, Name = "Creator" } },
                new GroupMember { UserId = 2, Role = GroupRole.Member, User = new User { Id = 2, Name = "Member" } }
            }
        };
        _groupRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(createdGroup);

        //Act
        var result = await _service.CreateAsync(dto);

        //Assert
        result.Should().NotBeNull();
        result.Members.Should().HaveCount(2);
        result.Members.Should().Contain(m => m.UserId == 1 && m.Role == GroupRole.Admin);
        result.Members.Should().Contain(m => m.UserId == 2 && m.Role == GroupRole.Member);

        _groupRepoMock.Verify(r => r.Add(It.IsAny<Group>()), Times.Once);
    }

    [Fact]
    public async Task AddMember_WhenUserNotAlreadyMember_ReturnsMember()
    {
        //Arrange
        var groupId = 1;
        var dto = new AddMemberDto { UserId = 2, Role = GroupRole.Member };

        _groupRepoMock.Setup(r => r.GetByIdBasicAsync(groupId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new Group { Id = groupId, Name = "Test Group", CreatedByUserId = 1 });

        _userRepoMock.Setup(r => r.GetByIdAsync(dto.UserId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new User { Id = 2, Name = "TestUser" });

        _groupRepoMock.Setup(r => r.IsMemberAsync(groupId, dto.UserId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        //Act
        var result = await _service.AddMemberAsync(groupId, dto);

        //Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(2);
        result.Name.Should().Be("TestUser");
        result.Role.Should().Be(GroupRole.Member);

        // Verify the add actually happened (interaction check, not just return value)
        _groupRepoMock.Verify(r => r.AddMember(It.IsAny<GroupMember>()), Times.Once);

    }

    [Fact]
    public async Task GetById_WhenGroupExist_ReturnsGroupWithMembers()
    {
        // Arrange
        var groupId = 1;
        var group = new Group
        {
            Id = groupId,
            Name = "Test Group",
            Description = "A test group",
            CreatedByUserId = 1,
            Members = new List<GroupMember>
            {
                new GroupMember { UserId = 1, Role = GroupRole.Admin, User = new User { Id = 1, Name = "Vasu"} },
                new GroupMember { UserId = 2, Role = GroupRole.Member, User = new User { Id = 2, Name = "Harshit"} }
            }
        };
        _groupRepoMock.Setup(r => r.GetByIdAsync(groupId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(group);
        // Act
        var result = await _service.GetByIdAsync(groupId);
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(groupId);
        result.Members.Should().HaveCount(2);
        result.Members.Should().Contain(m => m.UserId == 1 && m.Name == "Vasu" && m.Role == GroupRole.Admin);
        result.Members.Should().Contain(m => m.UserId == 2 && m.Name == "Harshit" && m.Role == GroupRole.Member);
    }

    
}
