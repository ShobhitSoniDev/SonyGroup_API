using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class CategoryMaster_ManageCommand : IRequest<ResponseModel>
    {
        public int MetalId { get; set; } = 0;
        public string CategoryName { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int TypeId { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
    }

    // ✅ Handler
    public class CategoryMaster_ManageCommandHandler
        : IRequestHandler<CategoryMaster_ManageCommand, ResponseModel>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        public CategoryMaster_ManageCommandHandler(ICategoryRepository categoryRepository,IErrorLogRepository errorLogRepository)
        {
            _categoryRepository = categoryRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<ResponseModel> Handle(CategoryMaster_ManageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.TypeId == 1 || request.TypeId == 2)
                {
                    var error = CommonInputValidator.Validate(value: request.CategoryName, numeric: false, minLength: 2, maxLength: 20);
                    if (error.Code == 0)
                        return error;

                    error = CommonInputValidator.Validate(value: request.MetalId.ToString(), numeric: true, minLength: 1, maxLength: 20);
                    if (error.Code == 0)
                        return error;
                }

                var insertedCategory = await _categoryRepository.CategoryMaster_ManageAsync(request.MetalId, request.CategoryName, request.CreatedBy, request.TypeId, request.CategoryId);
                if (insertedCategory != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = insertedCategory
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
            catch(Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                int? lineNumber = frame?.GetFileLineNumber();
                string? stackTraceText = ex.StackTrace;
                var errorLog = new ErrorLog
                {
                    ApiName = "CategoryMaster_ManageCommandHandler",
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
