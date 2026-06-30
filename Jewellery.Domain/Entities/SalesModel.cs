using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class SalesModel
    {
        public int SaleId { get; set; }
        public string EntryType { get; set; } = string.Empty;
        public string BillNo { get; set; } = string.Empty;
        public DateTime? BillDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerType { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public bool IsActive { get; set; } = true;
        // Sale Details
        public List<SalesDetailModel> Details { get; set; } = new();
        public List<OldJewelleryModel> OldJewelleryDetails { get; set; }
    }
    public class SalesDetailModel
    {
        public int SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string MetalType { get; set; } = "GOLD";   // GOLD (/10gm) or SILVER (/kg)
        public decimal Touch { get; set; }
        public decimal PureWeight { get; set; }
        public decimal MetalRate { get; set; }
        public decimal MakingCharge { get; set; }
        public string MakingChargeType { get; set; } = string.Empty;
        public decimal StoneCharge { get; set; }
        public decimal GSTRate { get; set; }
        public decimal Amount { get; set; }
    }
    public class OldJewelleryModel
    {
        public int OldJewelDetailId { get; set; }
        public int SaleId { get; set; }
        public string ItemDescription { get; set; }
        public decimal GrossWeight { get; set; }
        public string MetalType { get; set; } = "GOLD";   // GOLD (/10gm) or SILVER (/kg)
        public decimal? Touch { get; set; }             // HOLESALE only
        public decimal? DeductionWeight { get; set; }    // HOLESALE only
        public decimal? PureWeight { get; set; }         // HOLESALE only
        public decimal MetalRate { get; set; }
        public decimal Amount { get; set; }
        public bool? IsActive { get; set; }
    }
}
