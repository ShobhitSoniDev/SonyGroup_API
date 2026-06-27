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
        public string BillNo { get; set; } = string.Empty;
        public DateTime? BillDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int TypeId { get; set; }

        // Sale Details
        public List<SalesDetailModel> Details { get; set; } = new();
    }
    public class SalesDetailModel
    {
        public int SaleDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public decimal Rate { get; set; }
        public decimal MakingCharge { get; set; }
        public decimal Amount { get; set; }
    }
}
