using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ICustomerRepository
    {
        Task<dynamic> ProductMaster_ManageAsync(CustomerMasterModel customer);
    }
}

