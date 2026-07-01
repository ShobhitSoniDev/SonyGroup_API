using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class LoanTransactionsDetailModel
    {
        public long LoanTransactionId { get; set; }

        public long LoanId { get; set; }

        public int? TransactionTypeId { get; set; }
        public int? InterestRate { get; set; }

        public string? TransactionDate { get; set; }

        public decimal Amount { get; set; }

        public string? Description { get; set; }

        public string TypeId { get; set; } = string.Empty;
    }
}
