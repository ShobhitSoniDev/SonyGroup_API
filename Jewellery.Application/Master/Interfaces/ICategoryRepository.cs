using Jewellery.Application.Master.Models;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ICategoryRepository
    {
        Task<dynamic> CategoryMaster_ManageAsync(int MetalId, string CategoryName, string createdBy,int TypeId, int CategoryId);
    }
}

