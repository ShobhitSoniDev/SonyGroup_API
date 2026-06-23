using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetCustomerLedgerReportModel
    {
        public string? CustomerCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? TransactionType { get; set; }   // NULL=All | 1=DR | 2=CR
        public int TypeId { get; set; }   // 1=Detail | 2=Summary
    }
}
