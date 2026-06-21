using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniSplitwise.Api.DTOs;
using MiniSplitwise.Api.Models;
using MiniSplitwise.Api.Repositories;

namespace MiniSplitwise.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<List<UserResponseDto>> GetAllAsync(CancellationToken ct = default)
        {
            var users = await _userRepository.GetAllAsync(ct);
            return users.Select(MapToDto).ToList();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var user = await _userRepository.GetByIdAsync(id, ct);
            return user is not null ? MapToDto(user) : null;
        }

        public async Task<UserResponseDto> CreateAsync(UserCreateDto userCreateDto, CancellationToken ct = default)
        {
            // Business rule: emails must be unique
            var email = userCreateDto.Email.Trim().ToLowerInvariant(); 

            if (await _userRepository.EmailExistsAsync(email, ct))
            {
                _logger.LogWarning("Attempt to create user with existing email: {Email}", email);
                throw new InvalidOperationException($"A user with email '{email}' already exists.");
            }
            var user = new User
            {
                Name = userCreateDto.Name.Trim(),
                Email = email,
            };

            try
            {
                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync(ct);
            }
            catch(DbUpdateException ex)
                when(ex.InnerException is SqlException { Number: 2601 or 2627})
            {
                // Backstop: two concurrent requests both passed the check above,
                // and the unique index rejected the second insert.
                _logger.LogWarning("Unique index caught duplicate email on insert: {Email}", email);
                throw new InvalidOperationException($"A user with email '{email}' already exists.");
            }
            

            return MapToDto(user);
        }


        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
