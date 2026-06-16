using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class GetLoanEntryReportModel
    {
        public int? LoanId { get; set; }
        public int? CustomerId { get; set; }
        public string? LoanType { get; set; }
        public string? LoanStatus { get; set; }
        public string? MetalType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? AmountFrom { get; set; }
        public long? AmountTo { get; set; }
        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
