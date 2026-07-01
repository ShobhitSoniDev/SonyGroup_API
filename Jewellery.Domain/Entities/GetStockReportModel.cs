using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetStockReportModel
    {
        public DateTime? AsOnDate { get; set; }   // Stock as of this date (inclusive) | NULL = as of today
        public int? ProductId { get; set; }   // NULL = all products
        public int? CategoryId { get; set; }   // NULL = all categories
        public int? MetalId { get; set; }   // NULL = all metals (Product_Master.MetalId)
        public string? MetalType { get; set; }   // "GOLD" | "SILVER" | NULL = all
        public string? StockStatus { get; set; }   // "IN_STOCK" | "OUT_OF_STOCK" | "LOW_STOCK" | NULL = all
        public int? LowStockQty { get; set; }   // Threshold used when StockStatus = "LOW_STOCK"
        public bool? IsActive { get; set; }   // NULL = both active & inactive
    }
}
