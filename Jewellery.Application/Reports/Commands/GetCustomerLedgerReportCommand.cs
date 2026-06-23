using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jewellery.Application.Master.Commands
{
    // ─── Command ──────────────────────────────────────────────────────────────
    public class GetCustomerLedgerReportCommand : IRequest<ResponseModel>
    {
        public string? CustomerCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TransactionType { get; set; }   // NULL=All | 1=DR | 2=CR
        public int TypeId { get; set; }   // 1=Detail | 2=Summary
    }

    // ─── Handler ──────────────────────────────────────────────────────────────
    public class GetCustomerLedgerReportCommandHandler
        : IRequestHandler<GetCustomerLedgerReportCommand, ResponseModel>
    {
        private readonly IReportsRepository _reportsRepository;

        public GetCustomerLedgerReportCommandHandler(
            IReportsRepository reportsRepositoryRepository)
        {
            _reportsRepository = reportsRepositoryRepository;
        }

        public async Task<ResponseModel> Handle(
            GetCustomerLedgerReportCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var model = new GetCustomerLedgerReportModel
                {
                    CustomerCode = request.CustomerCode,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    TransactionType = request.TransactionType,
                    TypeId = request.TypeId
                };

                var result = await _reportsRepository
                                   .CustomerLedgerReportAsync(model);

                if (result != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = result
                    };
                }
                else
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "FAILED"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    Code = 0,
                    Message = ex.Message
                };
            }
        }
    }
}
