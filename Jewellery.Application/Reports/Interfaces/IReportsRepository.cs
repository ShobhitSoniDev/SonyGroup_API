using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IReportsRepository
    {
        Task<object> CustomerLedgerReportAsync(GetCustomerLedgerReportModel model);
    }
}

