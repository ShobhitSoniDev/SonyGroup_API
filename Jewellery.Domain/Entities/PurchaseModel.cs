using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class PurchaseModel
    {
        public int PurchaseId { get; set; }
        public string PurchaseNo { get; set; } = string.Empty;
        public DateTime? PurchaseDate { get; set; }
        public int SupplierId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public List<PurchaseDetailModel> DetailsJson { get; set; } = new();
        public List<PurchaseOldJewelleryModel>? OldJewelleryJson { get; set; } = new();   // ✅ NAYA
    }

    public class PurchaseDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string MetalType { get; set; } = "GOLD";     // ✅ NAYA — GOLD / SILVER
        public decimal MetalRate { get; set; }
        public decimal MakingCharge { get; set; }
        public string MakingChargeType { get; set; }
        public decimal StoneCharge { get; set; }
        public decimal Amount { get; set; }
    }

    /* ✅ NAYA — Old Jewellery Purchase ka model (Sales OldJewellery jaisa) */
    public class PurchaseOldJewelleryModel
    {
        public string? ItemDescription { get; set; }
        public decimal GrossWeight { get; set; }
        public string MetalType { get; set; } = "GOLD";     // GOLD / SILVER
        public decimal? Touch { get; set; }                 // Optional
        public decimal? DeductionWeight { get; set; }       // auto-calc (jab Touch diya ho)
        public decimal? PureWeight { get; set; }            // auto-calc (jab Touch diya ho)
        public decimal MetalRate { get; set; }
        public decimal Amount { get; set; }
    }
}
