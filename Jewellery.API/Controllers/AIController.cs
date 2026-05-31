using Jewellery.API.Filters;
using Jewellery.Application.Master.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using static AIService;

namespace Jewellery.API.Controllers.Master
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(ExceptionFilter))]
    [Route("api/[controller]")]
    public class AIController : BaseApiController
    {
        private readonly AIService _aiService;

        private readonly IMediator _mediator;
        public AIController(AIService aiService, IMediator mediator)
        {
            _aiService = aiService;
            _mediator = mediator;
        }
        // ✅ Chat API
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            var reply = await _aiService.GetChatResponse(request.Message);
            return Ok(new { reply });
        }
        [HttpPost("FAQMaster_Manage")]
        public async Task<IActionResult> FAQMaster_Manage([FromBody] FAQMaster_ManageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        
    }
}
