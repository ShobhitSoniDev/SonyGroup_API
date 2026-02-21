using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IProductRepository
    {
        Task<dynamic> ProductMaster_ManageAsync(ProductMasterModel product);
    }
}

