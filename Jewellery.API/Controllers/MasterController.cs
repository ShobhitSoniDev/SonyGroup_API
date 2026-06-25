using MediatR;
using Microsoft.AspNetCore.Mvc;
using Jewellery.Application.Master.Commands;
using Microsoft.AspNetCore.Authorization;
using Jewellery.API.Filters;

namespace Jewellery.API.Controllers.Master
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]")]
    public class MasterController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MasterController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("GetMenu")]
        public async Task<IActionResult> GetMenu()
        {
            return Ok(await Mediator.Send(new GetMenuQuery()));
        }

        [HttpPost("MetalMaster_Manage")]
        public async Task<IActionResult> MetalMaster_Manage([FromBody] MetalMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("CategoryMaster_Manage")]
        public async Task<IActionResult> CategoryMaster_Manage([FromBody] CategoryMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("ProductMaster_Manage")]
        public async Task<IActionResult> ProductMaster_Manage([FromBody] ProductMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("CustomerMaster_Manage")]
        public async Task<IActionResult> CustomerMaster_Manage([FromBody] CustomerMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("GetLoan_Masters")]
        public async Task<IActionResult> GetLoan_Masters()
        {
            return Ok(await Mediator.Send(new GetLoan_MastersQuery()));
        }
        [HttpPost("RoleMaster_Manage")]
        public async Task<IActionResult> RoleMaster_Manage([FromBody] RoleMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("RoleMenuMapping_Manage")]
        public async Task<IActionResult> RoleMenuMapping_Manage([FromBody] Role_Menu_Mapping_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("ChangePassword_Manage")]
        public async Task<IActionResult> ChangePassword_Manage([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("User_Manage")]
        public async Task<IActionResult> User_Manage([FromBody] User_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
