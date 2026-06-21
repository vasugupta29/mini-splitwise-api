using MiniSplitwise.Api.DTOs;

namespace MiniSplitwise.Api.Services
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync(CancellationToken ct = default);
        Task<UserResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<UserResponseDto> CreateAsync(UserCreateDto dto, CancellationToken ct = default);
    }
}
