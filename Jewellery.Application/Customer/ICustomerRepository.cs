using Jewellery.Domain.Entities;
using static Jewellery.Domain.Entities.CustomerOrderModel;

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
        Task<dynamic> CustomerAddress_ManageAndReturnAsync(CustomerAddressRequest model);



        // TypeId 1 = Place COD order, TypeId 2 = Create pending online order
        Task<dynamic> Order_PlaceAndReturnAsync(OrderPlaceRequest request);

        // Persist the RazorpayOrderId after creating it via Razorpay's API
        Task<dynamic> Order_UpdateRazorpayOrderIdAndReturnAsync(RazorpayOrderUpdateRequest request);

        // Persist verified/failed payment outcome (signature already checked in C#)
        Task<dynamic> Payment_VerifyAndReturnAsync(PaymentVerifyRequest request);

        // Optional — used by an order-confirmation screen
        Task<dynamic> Order_GetByIdAsync(int orderId);
        Task<OrderManageResult> Order_ManageAndReturnAsync(OrderManageRequest request);
    }
}

