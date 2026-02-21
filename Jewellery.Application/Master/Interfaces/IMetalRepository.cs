using Jewellery.Application.Master.Models;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IMetalRepository
    {
        Task<dynamic> MetalMaster_ManageAndReturnAsync(string metalName, int purity, string createdBy,int TypeId,int MetalId);

    }  
}

