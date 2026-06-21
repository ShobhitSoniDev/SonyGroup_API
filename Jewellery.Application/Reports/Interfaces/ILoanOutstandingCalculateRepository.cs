using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ILoanOutstandingCalculateRepository
    {
        
        Task<dynamic> LoanOutstandingCalculateAsync(int LoanId, DateTime CloserDate);
    }
}

