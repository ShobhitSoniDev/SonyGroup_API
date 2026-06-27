using MediatR;
using Microsoft.AspNetCore.Mvc;
using Jewellery.Application.Master.Commands;
using Microsoft.AspNetCore.Authorization;
using Jewellery.API.Filters;
using Jewellery.Application.Transactions.Commands;

namespace Jewellery.API.Controllers.Transactions
{
    [Authorize]
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
        [HttpPost("LoanEntry_Save")]
        public async Task<IActionResult> LoanEntry_Save([FromForm] LoanEntry_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("LoanTransactionsDetail_Manage")]
        public async Task<IActionResult> LoanTransactionsDetail_Manage([FromBody] LoanTransactionsDetail_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("CustomerLedger_Manage")]
        public async Task<IActionResult> CustomerLedger_Manage([FromBody] CustomerLedger_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Sales_Manage")]
        public async Task<IActionResult> Sales_Manage([FromBody] Sales_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("Purchase_Manage")]
        public async Task<IActionResult> Purchase_Manage([FromBody] Purchase_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
