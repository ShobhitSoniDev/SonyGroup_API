using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class CustomerLedgerModel
    {
        public long TransId { get; set; }
        public string CustomerCode { get; set; }
        public string TransactionDate { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int TypeId { get; set; }
        public string UserId { get; set; }
    }
}
