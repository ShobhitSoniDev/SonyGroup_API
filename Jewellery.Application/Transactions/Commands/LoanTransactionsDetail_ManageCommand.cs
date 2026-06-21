using Jewellery.Application.Auth.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Services.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO.IsolatedStorage;

namespace Jewellery.Application.Transactions.Commands
{
    public class LoanTransactionsDetail_ManageCommand : IRequest<ResponseModel>
    {
        public long LoanTransactionId { get; set; }
        public long LoanId { get; set; }
        public int? TransactionTypeId { get; set; }
        public int? InterestRate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string TypeId { get; set; } = string.Empty;
    }
    public class LoanTransactionsDetail_ManageCommandHandler
    : IRequestHandler<LoanTransactionsDetail_ManageCommand, ResponseModel>
    {
        private readonly ILoanRepository _loanRepository;

        public LoanTransactionsDetail_ManageCommandHandler(
            ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<ResponseModel> Handle(
            LoanTransactionsDetail_ManageCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                if ((request.TypeId == "1" || request.TypeId == "2")
                    && request.LoanId <= 0)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Invalid LoanId"
                    };
                }

                if ((request.TypeId == "1" || request.TypeId == "2")
                    && request.TransactionDate == null)
                {
                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Transaction Date is required"
                    };
                }

                var model = new LoanTransactionsDetailModel
                {
                    LoanTransactionId = request.LoanTransactionId,
                    LoanId = request.LoanId,
                    TransactionTypeId = request.TransactionTypeId,
                    InterestRate = request.InterestRate,
                    TransactionDate = request.TransactionDate?.ToString("yyyy-MM-dd"),
                    Amount = request.Amount,
                    Description = request.Description,
                    CreatedBy = request.CreatedBy,
                    TypeId = request.TypeId
                };

                var result = await _loanRepository
                    .LoanTransactionsDetail_ManageAsync(model);

                return new ResponseModel
                {
                    Code = result != null ? 1 : 0,
                    Message = result != null ? "SUCCESS" : "FAILED",
                    Data = result
                };
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