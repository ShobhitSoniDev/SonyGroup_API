using Jewellery.API.Filters;
using Jewellery.Application.Master.Commands;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewellery.API.Controllers.Master
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]")]
    public class CustomerController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Customer_Cart_Manage")]
        public async Task<IActionResult> Customer_Cart_Manage([FromBody] Customer_Cart_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Online_Product_Manage")]
        public async Task<IActionResult> Online_Product_Manage([FromBody] Online_Product_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Product_Images_Manage")]
        public async Task<IActionResult> Product_Images_Manage([FromForm] ProductImages_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
