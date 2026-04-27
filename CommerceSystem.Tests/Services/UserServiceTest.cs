using Xunit;
using Moq;
using CommerceSystem.Api.Services;
using CommerceSystem.Api.Repositories;
using CommerceSystem.Api.Models;
using CommerceSystem.Api.DTOs;

namespace CommerceSystem.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUserAsync_ValidRequest_CreatesUser()
    {
        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var service = new UserService(repo.Object);

        var request = new CreateUserRequest
        {
            Name = "Test",
            Email = "test@test.com"
        };

        var result = await service.CreateUserAsync(request);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
    {
        var repo = new Mock<IUserRepository>();

        repo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User { Id = 1, Name = "Test" });

        var service = new UserService(repo.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }
}