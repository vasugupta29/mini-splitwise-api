using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniSplitwise.Api.DTOs;
using MiniSplitwise.Api.Services;

namespace MiniSplitwise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        { 
            _userService = userService;
        }

        ///<summary>Returns all users.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDto>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var users = await _userService.GetAllAsync(ct);
            return Ok(users);
        }

        ///<summary>Returns a single user by ID.</summary>
        [HttpGet("{id:int}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var user = await _userService.GetByIdAsync(id, ct);
            if (user is null)
                return NotFound(new {message = $"User with id {id} not found." });

            return Ok(user);
        }

        ///<summary>Creates a new user.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(UserCreateDto dto, CancellationToken ct)
        {
            try
            {
                var createdUser = await _userService.CreateAsync(dto, ct);
                return CreatedAtRoute("GetUserById", new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
