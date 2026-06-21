using MiniSplitwise.Api.Models;

namespace MiniSplitwise.Api.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(CancellationToken ct = default);
        Task<User?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        void Add(User user);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<List<int>> GetExistingUserIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
