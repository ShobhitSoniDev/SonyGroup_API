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
    public class ReportsController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("GetLoanEntry")]
        public async Task<IActionResult> GetLoanEntryReport([FromBody] GetLoanEntryReportCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
