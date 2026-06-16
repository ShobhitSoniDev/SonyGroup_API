using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ILoanEntryReportRepository
    {
        Task<dynamic> LoanEntryReportsAsync(GetLoanEntryReportModel customer);
    }
}

