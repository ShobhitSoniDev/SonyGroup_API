using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using System.Threading;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;
using Jewellery.Application.Auth.Interfaces;

namespace Jewellery.Application.Reports.Queries
{
    // ✅ Command
    public class Dashboard_GetDataQuery : IRequest<ResponseModel>
    {

    }

    // ✅ Handler
    public class Dashboard_GetDataQueryHandler
        : IRequestHandler<Dashboard_GetDataQuery, ResponseModel>
    {
        private readonly IDashboard_GetDataRepository _dashboard_GetDataRepository;

        public Dashboard_GetDataQueryHandler(IDashboard_GetDataRepository dashboard_GetDataRepository)
        {
            _dashboard_GetDataRepository = dashboard_GetDataRepository;
        }

        public async Task<ResponseModel> Handle(Dashboard_GetDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var GetDashboardData = await _dashboard_GetDataRepository.Dashboard_GetDataAsync();
                if (GetDashboardData != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = GetDashboardData
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
