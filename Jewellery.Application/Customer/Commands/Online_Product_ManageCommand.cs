using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class Online_Product_ManageCommand : IRequest<ResponseModel>
    {
        public int TypeId { get; set; }
        public int ProductId { get; set; }

        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public bool? IsFeatured { get; set; }
        public bool? ShowOnWeb { get; set; }
    }

    // ✅ Handler
    public class Online_Product_ManageCommandHandler
    : IRequestHandler<Online_Product_ManageCommand, ResponseModel>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ICloudinaryStorageService _cloudinaryStorageService;
        public Online_Product_ManageCommandHandler(
            ICustomerRepository customerRepository,
            IErrorLogRepository errorLogRepository,
            ICloudinaryStorageService cloudinaryStorageService)
        {
            _customerRepository = customerRepository;
            _errorLogRepository = errorLogRepository;
            _cloudinaryStorageService = cloudinaryStorageService;
        }

        public async Task<ResponseModel> Handle(
            Online_Product_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new OnlineProductManageRequestModel
                {
                    TypeId = request.TypeId,
                    ProductId = request.ProductId,
                    ShortDescription = request.ShortDescription,
                    LongDescription = request.LongDescription,
                    IsFeatured = request.IsFeatured,
                    ShowOnWeb = request.ShowOnWeb
                };

                var result = await _customerRepository
                    .Online_Product_ManageAsync(model);
                if (request.TypeId == 3 && result != null && result.Count > 0)
                {
                    foreach (var productimage in (IEnumerable<dynamic>)result)
                    {
                        if (!string.IsNullOrWhiteSpace(productimage.PrimaryImage))
                        {
                            var files = _cloudinaryStorageService.GetFileUrl(productimage.PrimaryImage.Trim());
                            ((IDictionary<string, object>)productimage)["PrimaryImageUrl"] = files;
                        }
                    }
                }
                if (result != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
                    };
                }

                return new ResponseModel
                {
                    Code = 0,
                    Message = "FAILED"
                };
            }
            catch (Exception ex)
            {
                var stackTrace = new StackTrace(ex, true);
                var frame = stackTrace.GetFrame(0);

                var errorLog = new ErrorLog
                {
                    ApiName = "Online_Product_ManageCommand",
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    LineNumber = frame?.GetFileLineNumber() ?? 0,
                    CreatedDate = DateTime.Now
                };

                await _errorLogRepository.SaveErrorAsync(errorLog);

                return new ResponseModel
                {
                    Code = 0,
                    Message = "Something went wrong. Please try again later."
                };
            }
        }
    }
}
