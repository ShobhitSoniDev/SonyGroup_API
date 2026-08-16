using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetOnlineProductListModel
    {
        public int? CategoryId { get; set; }

        public int? MetalId { get; set; }

        public string? SearchText { get; set; }

        public bool? OnlyFeatured { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
    public class OnlineProductDetailResult
    {
        public dynamic? Product { get; set; }

        public dynamic? Images { get; set; }
    }
    public class CustomerWishlistManageModel
    {
        public int TypeId { get; set; }

        public int? ProductId { get; set; }
    }
    public class CustomerWishlistManageResult
    {
        public bool IsAdded { get; set; }

        public dynamic? Wishlist { get; set; }
    }
    public class CustomerAddressRequest
    {
        public int TypeId { get; set; }

        public int? AddressId { get; set; }

        public string? AddressLabel { get; set; }

        public string? AddressLine { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }

        public bool IsDefault { get; set; }
    }
}
