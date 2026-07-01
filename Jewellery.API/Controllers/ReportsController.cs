using Jewellery.API.Filters;
using Jewellery.Application.Master.Commands;
using Jewellery.Application.Reports.Queries;
using Jewellery.Application.Transactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewellery.API.Controllers.Transactions
{
    //[Authorize]
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
        [HttpPost("CustomerBillGenerate")]
        public async Task<IActionResult> CustomerBillGenerate([FromBody] CustomerBillGenerateCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("GetPurchaseReport")]
        public async Task<IActionResult> GetPurchaseReport([FromBody] GetPurchaseReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("GetSaleReport")]
        public async Task<IActionResult> GetSaleReport([FromBody] GetSalesReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost("GetStock_Report")]
        public async Task<IActionResult> GetStock_Report([FromBody] GetStockReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
