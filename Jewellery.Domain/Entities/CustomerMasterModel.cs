using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class CustomerMasterModel
    {
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public int Pincode { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }
    public class CartItemResponse
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? CurrentRate { get; set; }
        public decimal? GrossWeight { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? MakingCharge { get; set; }
        public string? MakingChargeType { get; set; }
        public string? PrimaryImage { get; set; }
    }
    public class CartManageRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int TypeId { get; set; } = 0;
    }
}
