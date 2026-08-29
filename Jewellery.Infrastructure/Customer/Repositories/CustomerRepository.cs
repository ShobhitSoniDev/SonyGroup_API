using Dapper;
using Jewellery.Application.Common.Interfaces;
using Jewellery.Application.Master.Interfaces;
using Jewellery.Application.Transactions.Interfaces;
using Jewellery.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using static Jewellery.Domain.Entities.CustomerOrderModel;

namespace Jewellery.Infrastructure.Transactions.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ICurrentUserService _currentUser;

        public CustomerRepository(
            IConfiguration configuration,
            ICurrentUserService currentUser)
        {
            _configuration = configuration;
            _currentUser = currentUser;
        }
        public async Task<dynamic> Customer_Cart_ManageAsync(CartManageRequest request)
        {
            using var connection = new SqlConnection(
               _configuration.GetConnectionString(_currentUser.shopCode));
            var parameters = new DynamicParameters();
            parameters.Add("@TypeId", request.TypeId);
            parameters.Add("@CustomerCode", _currentUser.UserId);
            parameters.Add("@ProductId", request.ProductId, DbType.Int32);
            parameters.Add("@Quantity", request.Quantity, DbType.Int32);

            var result = await connection.QueryAsync(
                "Jewellery.Customer_Cart_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<dynamic> CustomerLedger_ManageAsync(CustomerLedgerModel model)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TransId", model.TransId);
            parameters.Add("@CustomerCode", model.CustomerCode);
            parameters.Add("@TransactionDate", model.TransactionDate);
            parameters.Add("@TransactionType", model.TransactionType);
            parameters.Add("@Amount", model.Amount);
            parameters.Add("@Description", model.Description);
            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@UserId", _currentUser.UserId);

            var result = await connection.QueryAsync(
                "Jewellery.CustomerLedger_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<dynamic> Online_Product_ManageAsync(
    OnlineProductManageRequestModel request)
        {
            using var connection = new SqlConnection(
                _configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", request.TypeId, DbType.Int32);
            parameters.Add("@ProductId", request.ProductId, DbType.Int32);
            parameters.Add("@ShortDescription", request.ShortDescription, DbType.String);
            parameters.Add("@LongDescription", request.LongDescription, DbType.String);
            parameters.Add("@IsFeatured", request.IsFeatured, DbType.Boolean);
            parameters.Add("@ShowOnWeb", request.ShowOnWeb, DbType.Boolean);

            var result = await connection.QueryAsync(
                "Jewellery.Online_Product_Manage",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

    public async Task<dynamic> Product_Images_ManageAsync(ProductImagesManageModel model)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();

            parameters.Add("@TypeId", model.TypeId, DbType.Int32);
            parameters.Add("@ProductId", model.ProductId, DbType.Int32);
            parameters.Add("@ImageId", model.ImageId, DbType.Int32);
            parameters.Add("@ImagePath", model.ImagePath,  DbType.String);
            parameters.Add("@IsPrimary", model.IsPrimary, DbType.Boolean);
            parameters.Add("@DisplayOrder", model.DisplayOrder, DbType.Int32);
            var result=await connection.QueryAsync("Jewellery.Product_Images_Manage",parameters,commandType:CommandType.StoredProcedure);
            return result;
        }

        public async Task<dynamic> GetOnline_ProductListAsync(GetOnlineProductListModel model)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", model.CategoryId, DbType.Int32);
            parameters.Add("@MetalId", model.MetalId, DbType.Int32);
            parameters.Add("@SearchText", model.SearchText, DbType.String);
            parameters.Add("@OnlyFeatured", model.OnlyFeatured, DbType.Boolean);
            parameters.Add("@PageNumber", model.PageNumber, DbType.Int32);
            parameters.Add("@PageSize", model.PageSize, DbType.Int32);

            var result = await connection.QueryAsync(
                "Jewellery.GetOnline_ProductList",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }
        public async Task<OnlineProductDetailResult?> GetOnline_ProductByProductIdAsync(int ProductId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@ProductId", ProductId, DbType.Int32);

            using var multi = await connection.QueryMultipleAsync(
                "Jewellery.GetOnline_ProductByProductId",
                parameters,
                commandType: CommandType.StoredProcedure);

            // 1st result set -> Product details (single row)
            var product = await multi.ReadFirstOrDefaultAsync();

            // 2nd result set -> Images list
            var images = await multi.ReadAsync();

            if (product == null)
            {
                return null;
            }

            return new OnlineProductDetailResult
            {
                Product = product,
                Images = images
            };
        }
        public async Task<CustomerWishlistManageResult> Customer_Wishlist_ManageAsync(CustomerWishlistManageModel model)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@TypeId", model.TypeId, DbType.Int32);
            parameters.Add("@CustomerCode", _currentUser.UserId);
            parameters.Add("@ProductId", model.ProductId, DbType.Int32);
            parameters.Add("@IsAdded", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            if (model.TypeId == 2)
            {
                // Result set aayega (wishlist list)
                var wishlist = await connection.QueryAsync(
                    "Jewellery.Customer_Wishlist_Manage",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return new CustomerWishlistManageResult
                {
                    IsAdded = false,
                    Wishlist = wishlist
                };
            }
            else
            {
                // Sirf Add/Remove -> koi result set nahi, ExecuteAsync se output param milega
                await connection.ExecuteAsync(
                    "Jewellery.Customer_Wishlist_Manage",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                bool isAdded = parameters.Get<bool>("@IsAdded");

                return new CustomerWishlistManageResult
                {
                    IsAdded = isAdded,
                    Wishlist = null
                };
            }
        }
        public async Task<dynamic> CustomerAddress_ManageAndReturnAsync(CustomerAddressRequest model)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
            var parameters = new DynamicParameters();
            parameters.Add("@TypeId", model.TypeId);
            parameters.Add("@AddressId", model.AddressId);
            parameters.Add("@CustomerCode", _currentUser.UserId);
            parameters.Add("@AddressLabel", model.AddressLabel);
            parameters.Add("@AddressLine", model.AddressLine);
            parameters.Add("@MobileNo", model.MobileNo);
            parameters.Add("@City", model.City);
            parameters.Add("@State", model.State);
            parameters.Add("@Pincode", model.Pincode);
            parameters.Add("@IsDefault", model.IsDefault);

            // Stored Procedure MUST return SELECT
            return await connection.QueryAsync("Jewellery.ManageCustomer_Address", parameters, commandType: CommandType.StoredProcedure);
        }

        #region Customer Order

        public async Task<dynamic> Order_PlaceAndReturnAsync(OrderPlaceRequest request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@TypeId", request.TypeId);
            parameters.Add("@CustomerCode", _currentUser.UserId);
            parameters.Add("@AddressId", request.AddressId);
            parameters.Add("@PaymentMode", request.PaymentMode);

            var result = await connection.QueryFirstOrDefaultAsync(
                "Jewellery.Customer_Order_PlaceAndReturn",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<dynamic> Order_UpdateRazorpayOrderIdAndReturnAsync(RazorpayOrderUpdateRequest request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", request.OrderId);
            parameters.Add("@RazorpayOrderId", request.RazorpayOrderId);

            var result = await connection.QueryFirstOrDefaultAsync(
                "Jewellery.Customer_Order_UpdateRazorpayOrderId",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<dynamic> Payment_VerifyAndReturnAsync(PaymentVerifyRequest request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", request.OrderId);
            parameters.Add("@RazorpayOrderId", request.RazorpayOrderId);
            parameters.Add("@RazorpayPaymentId", request.RazorpayPaymentId);
            parameters.Add("@RazorpaySignature", request.RazorpaySignature);
            parameters.Add("@IsValid", request.IsValid);

            var result = await connection.QueryFirstOrDefaultAsync(
                "Jewellery.Customer_Payment_VerifyAndReturn",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<dynamic> Order_GetByIdAsync(int orderId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));

            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", orderId);

            using var multi = await connection.QueryMultipleAsync(
                "Jewellery.Customer_Order_GetById",
                parameters,
                commandType: CommandType.StoredProcedure);

            var order = await multi.ReadFirstOrDefaultAsync();
            var items = await multi.ReadAsync();

            return new { Order = order, Items = items };
        }
        public async Task<OrderManageResult> Order_ManageAndReturnAsync(OrderManageRequest request)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString(_currentUser.shopCode));
            var parameters = new DynamicParameters();
            parameters.Add("@TypeId", request.TypeId);
            parameters.Add("@CustomerCode", _currentUser.UserId);
            parameters.Add("@OrderId", request.OrderId);

            if (request.TypeId == 1)
            {
                using var multi = await connection.QueryMultipleAsync(
                    "Jewellery.Customer_Order_Manage", parameters, commandType: CommandType.StoredProcedure);

                var order = await multi.ReadFirstOrDefaultAsync();

                if (order == null || (int)order.Code != 1)
                {
                    return new OrderManageResult { Code = 0, Message = (string)(order?.Message ?? "Order not found") };
                }

                var items = await multi.ReadAsync();
                return new OrderManageResult { Code = 1, Message = "SUCCESS", Order = order, Items = items };
            }
            else
            {
                var rows = await connection.QueryAsync(
                    "Jewellery.Customer_Order_Manage", parameters, commandType: CommandType.StoredProcedure);
                return new OrderManageResult { Code = 1, Message = "SUCCESS", Data = rows };
            }
        }
        #endregion
    }
}