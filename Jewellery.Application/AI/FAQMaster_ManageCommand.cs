using Jewellery.Application.Master.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class FAQMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int Id { get; set; } = 0;

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public string Keywords { get; set; } = string.Empty;

        public string SearchText { get; set; } = string.Empty;

        public int TypeId { get; set; } = 0;
    }

    // ✅ Handler
    public class FAQMaster_ManageCommandHandler
     : IRequestHandler<FAQMaster_ManageCommand, ResponseModel>
    {
        private readonly IFAQMasterRepository _faqMasterRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public FAQMaster_ManageCommandHandler(IFAQMasterRepository faqMasterRepository, IErrorLogRepository errorLogRepository)
        {
            _faqMasterRepository = faqMasterRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(FAQMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔥 INSERT + RETURN ROW (dynamic)
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    var error = CommonInputValidator.Validate(value: request.Question, numeric: false, minLength: 2, maxLength: 20);
                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(value: request.Answer.ToString(), numeric: false, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                }
                var faqModel = new FAQMasterModel
                {
                    Id = 0,
                    Question = string.Empty,
                    Answer = string.Empty,
                    Keywords = string.Empty,
                    SearchText = request.SearchText,
                    TypeId = 5
                };
                var insertedFAQ = await _faqMasterRepository.FAQMaster_ManageAndReturnAsync(faqModel);
                if (insertedFAQ != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = insertedFAQ
                    };
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "FAILED"
                    };
                }
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "FAQMaster_ManageCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = stackTraceText,
                    LineNumber = lineNumber ?? 0,
                    CreatedDate = DateTime.Now
                };
                // ✅ Save Log in DB (via Infrastructure)
                _errorLogRepository.SaveErrorAsync(errorLog);
                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }
    }

}
