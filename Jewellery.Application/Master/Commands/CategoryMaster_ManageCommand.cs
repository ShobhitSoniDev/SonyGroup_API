using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using System.Threading;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;

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

        public CategoryMaster_ManageCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
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
                return new ResponseModel
                {
                    Code = 1,
                    Message = "FAILED"
                };
            }
        }
    }
}
