using Jewellery.API.Filters;
using Jewellery.Application.Master.Commands;
using Jewellery.Application.Master.Queries;
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
        [HttpPost("GetOnline_ProductList")]
        public async Task<IActionResult> GetOnline_ProductList([FromBody] GetOnline_ProductListCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("GetOnline_ProductByProductId")]
        public async Task<IActionResult> GetOnline_ProductByProductId([FromQuery] GetOnline_ProductByProductIdQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("Customer_Wishlist_Manage")]
        public async Task<IActionResult> Customer_Wishlist_Manage([FromBody] CustomerWishlist_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
