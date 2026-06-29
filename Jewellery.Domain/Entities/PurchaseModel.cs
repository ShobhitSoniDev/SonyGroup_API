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
    }
    public class PurchaseDetailModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public decimal MetalRate { get; set; }
        public decimal MakingCharge { get; set; }
        public string MakingChargeType { get; set; }
        public decimal StoneCharge { get; set; }
        public decimal Amount { get; set; }
    }
}
