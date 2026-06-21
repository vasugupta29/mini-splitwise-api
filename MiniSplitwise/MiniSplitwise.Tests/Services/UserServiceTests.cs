using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using MiniSplitwise.Api.Models;
using MiniSplitwise.Api.Repositories;
using MiniSplitwise.Api.Services;
using Moq;

namespace MiniSplitwise.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task GetAll_WhenNoUsers_ReturnsEmptyList()
    {
        // Arrange
        var repoMock = new Mock<IUserRepository>();
        repoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());   // repository reports no users

        var service = new UserService(repoMock.Object, NullLogger<UserService>.Instance);

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}