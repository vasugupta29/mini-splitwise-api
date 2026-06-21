using System.ComponentModel.DataAnnotations;

namespace MiniSplitwise.Api.DTOs
{
    public class UserCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = null!;
    }
}
