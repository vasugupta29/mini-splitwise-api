using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniSplitwise.Api.DTOs;
using MiniSplitwise.Api.Exceptions;
using MiniSplitwise.Api.Services;

namespace MiniSplitwise.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        ///<summary>Creates a new group.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(GroupResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(GroupCreateDto dto, CancellationToken ct)
        {
            try
            {
                var createdGroup = await _groupService.CreateAsync(dto, ct);
                return CreatedAtRoute("GetGroupById", new { id = createdGroup.Id }, createdGroup);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}", Name = "GetGroupById")]
        [ProducesResponseType(typeof(GroupResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var group = await _groupService.GetByIdAsync(id, ct);
            return group is null ? NotFound(new { message = $"Group with id {id} not found." }) : Ok(group);
        }
    }
}
