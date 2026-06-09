using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface ICustomerRepository
    {
        Task<dynamic> CustomerMaster_ManageAsync(CustomerMasterModel customer);
    }
}

