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
    public class GetLoan_MastersQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class GetLoan_MastersQueryHandler
        : IRequestHandler<GetLoan_MastersQuery, ResponseModel>
    {
        private readonly IGetLoan_MastersRepository _getLoan_MastersRepository;

        public GetLoan_MastersQueryHandler(IGetLoan_MastersRepository getLoan_MastersRepository)
        {
            _getLoan_MastersRepository = getLoan_MastersRepository;
        }

        public async Task<ResponseModel> Handle(GetLoan_MastersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetLoan_Masters = await _getLoan_MastersRepository.GetLoan_MastersAsync();
                if (GetLoan_Masters != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetLoan_Masters
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
