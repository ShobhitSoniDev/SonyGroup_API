using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ILoanRepository
    {
        Task<dynamic> LoanEntry_ManageAsync(LoanEntryModel customer);
    }
}

