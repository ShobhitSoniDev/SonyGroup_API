using Jewellery.Domain.Entities;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface ICustomerRepository
    {
        Task<dynamic> Customer_Cart_ManageAsync(CartManageRequest request);
        Task<dynamic> Online_Product_ManageAsync(OnlineProductManageRequestModel request);
        Task<dynamic> Product_Images_ManageAsync(ProductImagesManageModel model);
    }
}

