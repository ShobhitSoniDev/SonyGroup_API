using Jewellery.Application.Master.Models;
using Jewellery.Domain.Entities;

namespace Jewellery.Application.Master.Interfaces
{
    public interface IMasterRepository
    {
        Task<dynamic> RoleMaster_ManageAsync(RoleMasterModel roleModel);
        Task<dynamic> RoleMenuMapping_ManageAsync(RoleMenuMappingModel roleModel);
        Task<dynamic> ChangePasswordAsync(ChangePasswordModel model);
        Task<dynamic> User_ManageAsync(UserModel model);
        Task<dynamic> Supplier_ManageAsync(SupplierModel model);
        Task<dynamic> ProductMaster_ManageAsync(ProductMasterModel product);
    }  
}

