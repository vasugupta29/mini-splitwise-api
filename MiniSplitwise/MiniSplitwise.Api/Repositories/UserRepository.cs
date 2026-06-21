using MiniSplitwise.Api.Data;
using MiniSplitwise.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MiniSplitwise.Api.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().OrderBy(u => u.Id).ToListAsync(ct); // read-only; skip change tracking for performance
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(u => u.Email == email, ct);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);
        }

        public async Task<List<int>> GetExistingUserIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
        {
            return await _context.Users.AsNoTracking()
                .Where(u => ids.Contains(u.Id))
                .Select(u => u.Id)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}