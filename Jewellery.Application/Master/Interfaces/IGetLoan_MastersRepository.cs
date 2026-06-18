using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IGetLoan_MastersRepository
    {
        Task<dynamic> GetLoan_MastersAsync();
    }
}

