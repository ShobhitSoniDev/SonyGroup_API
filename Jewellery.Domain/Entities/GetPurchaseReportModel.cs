using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetPurchaseReportModel
    {
        public DateTime? FromDate { get; set; }   // @FromDate
        public DateTime? ToDate { get; set; }   // @ToDate
        public int? SupplierId { get; set; }   // @SupplierId
        public int? ProductId { get; set; }   // @ProductId
        public int? CategoryId { get; set; }   // @CategoryId
        public int? MetalId { get; set; }   // @MetalId      (Product_Master.MetalId)
        public string? MetalType { get; set; }   // @MetalType    "GOLD" | "SILVER" | NULL
        public string? PurchaseNo { get; set; }   // @PurchaseNo   partial match
        public string? PaymentStatus { get; set; }   // @PaymentStatus "PAID" | "PARTIAL" | "UNPAID" | NULL
        public bool? IsActive { get; set; }   // @IsActive     NULL = both
    }
}
