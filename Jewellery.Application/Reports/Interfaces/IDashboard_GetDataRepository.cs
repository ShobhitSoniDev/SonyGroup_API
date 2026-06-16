using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IDashboard_GetDataRepository
    {
        Task<dynamic> Dashboard_GetDataAsync();
    }
}

