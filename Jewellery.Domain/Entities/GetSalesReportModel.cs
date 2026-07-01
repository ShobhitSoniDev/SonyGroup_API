using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetSalesReportModel
    {
        public DateTime? FromDate { get; set; }   // @FromDate
        public DateTime? ToDate { get; set; }   // @ToDate
        public int? CustomerId { get; set; }   // @CustomerId
        public string? CustomerType { get; set; }   // @CustomerType  "FULKAR" | "HOLESALE" | NULL
        public int? ProductId { get; set; }   // @ProductId
        public int? CategoryId { get; set; }   // @CategoryId
        public int? MetalId { get; set; }   // @MetalId       (Product_Master.MetalId)
        public string? MetalType { get; set; }   // @MetalType     "GOLD" | "SILVER" | NULL
        public string? BillNo { get; set; }   // @BillNo        partial match
        public string? PaymentMode { get; set; }   // @PaymentMode   "CASH"|"CARD"|"UPI"|"CHEQUE"|"MIXED"|NULL
        public string? PaymentStatus { get; set; }   // @PaymentStatus "PAID" | "PARTIAL" | "UNPAID" | NULL
        public bool? IsActive { get; set; }   // @IsActive      NULL = both
    }
}
