using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using System.Threading;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;
using Jewellery.Application.Auth.Interfaces;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class GetMenuQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class GetMenuQueryHandler
        : IRequestHandler<GetMenuQuery, ResponseModel>
    {
        private readonly IGetMenuRepository _getMenuRepository;

        public GetMenuQueryHandler(IGetMenuRepository categoryRepository)
        {
            _getMenuRepository = categoryRepository;
        }

        public async Task<ResponseModel> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetMenu = await _getMenuRepository.GetMenuReturnAsync();
                if (GetMenu != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetMenu
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
