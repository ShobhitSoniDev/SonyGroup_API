using MediatR;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Master.Models;
using System.Threading;
using System.Threading.Tasks;
using Jewellery.Domain.Entities;
using System;

namespace Jewellery.Application.Master.Commands
{
    // ✅ Command
    public class GetLoanEntryReportCommand : IRequest<ResponseModel>
    {
        public int? LoanId { get; set; }
        public int? CustomerId { get; set; }
        public string? LoanType { get; set; }
        public string? LoanStatus { get; set; }
        public string? MetalType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? AmountFrom { get; set; }
        public long? AmountTo { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    // ✅ Handler
    public class GetLoanEntryReportCommandHandler
        : IRequestHandler<GetLoanEntryReportCommand, ResponseModel>
    {
        private readonly ILoanEntryReportRepository _loanEntryReportsRepository;

        public GetLoanEntryReportCommandHandler(ILoanEntryReportRepository loanEntryReportsRepository)
        {
            _loanEntryReportsRepository = loanEntryReportsRepository;
        }

        public async Task<ResponseModel> Handle(GetLoanEntryReportCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var loanEntryReportModel = new GetLoanEntryReportModel
                {
                    LoanId = request.LoanId,
                    CustomerId = request.CustomerId,
                    LoanType = request.LoanType,
                    LoanStatus = request.LoanStatus,
                    MetalType = request.MetalType,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    AmountFrom = request.AmountFrom,
                    AmountTo = request.AmountTo,
                    PageNo = request.PageNo,
                    PageSize = request.PageSize
                };
                var getloanentry = await _loanEntryReportsRepository.LoanEntryReportsAsync(loanEntryReportModel);
                if (getloanentry != null)
                {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "SUCCESS",
                        Data = getloanentry
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
                return new ResponseModel
                {
                    Code = 1,
                    Message = "FAILED"
                };
            }
        }
    }
}
