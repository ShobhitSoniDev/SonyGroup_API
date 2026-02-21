using MediatR;
using Microsoft.AspNetCore.Mvc;
using Jewellery.Application.Master.Commands;
using Microsoft.AspNetCore.Authorization;
using Jewellery.API.Filters;
using Jewellery.Application.Transactions.Commands;

namespace Jewellery.API.Controllers.Transactions
{
    //[Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]")]
    public class TransactionsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("StockTransaction_Manage")]
        public async Task<IActionResult> StockTransaction_Manage([FromBody] StockTransaction_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
    }
}
