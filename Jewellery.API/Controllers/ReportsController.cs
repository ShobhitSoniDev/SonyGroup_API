using Jewellery.API.Filters;
using Jewellery.Application.Master.Commands;
using Jewellery.Application.Reports.Queries;
using Jewellery.Application.Transactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewellery.API.Controllers.Transactions
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]")]
    public class ReportsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Dashboard_GetData")]
        public async Task<IActionResult> Dashboard_GetData()
        {
            return Ok(await Mediator.Send(new Dashboard_GetDataQuery()));
        }

        [HttpPost("GetLoanEntry")]
        public async Task<IActionResult> GetLoanEntryReport([FromBody] GetLoanEntryReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("LoanOutstandingCalculate")]
        public async Task<IActionResult> LoanOutstandingCalculate([FromBody] LoanOutstandingCalculateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("CustomerLedgerReport")]
        public async Task<IActionResult> CustomerLedgerReport([FromBody] GetCustomerLedgerReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
