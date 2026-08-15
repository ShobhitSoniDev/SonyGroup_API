using Jewellery.Domain.Entities;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface ICustomerRepository
    {
        Task<dynamic> Customer_Cart_ManageAsync(CartManageRequest request);
        Task<dynamic> Online_Product_ManageAsync(OnlineProductManageRequestModel request);
        Task<dynamic> Product_Images_ManageAsync(ProductImagesManageModel model);
        Task<dynamic> GetOnline_ProductListAsync(GetOnlineProductListModel model);
        Task<OnlineProductDetailResult> GetOnline_ProductByProductIdAsync(int ProductId);
        Task<CustomerWishlistManageResult> Customer_Wishlist_ManageAsync(CustomerWishlistManageModel model);
    }
}

