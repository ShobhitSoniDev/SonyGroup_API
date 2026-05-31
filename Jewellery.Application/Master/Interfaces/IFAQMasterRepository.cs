using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IFAQMasterRepository
    {
        Task<dynamic> FAQMaster_ManageAndReturnAsync(FAQMasterModel model);

    }  
}

