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
    public class LoanOutstandingCalculateCommand : IRequest<ResponseModel>
    {
        public int LoanId { get; set; }
        public DateTime CloserDate { get; set; }
    }

    // ✅ Handler
    public class LoanOutstandingCalculateCommandHandler
        : IRequestHandler<LoanOutstandingCalculateCommand, ResponseModel>
    {
        private readonly ILoanOutstandingCalculateRepository _loanOutstandingCalculateRepository;

        public LoanOutstandingCalculateCommandHandler(ILoanOutstandingCalculateRepository loanOutstandingCalculateRepository)
        {
            _loanOutstandingCalculateRepository = loanOutstandingCalculateRepository;
        }

        public async Task<ResponseModel> Handle(LoanOutstandingCalculateCommand request, CancellationToken cancellationToken)
        {
            try
            {

                
                var getloanentry = await _loanOutstandingCalculateRepository.LoanOutstandingCalculateAsync(request.LoanId, request.CloserDate);
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
