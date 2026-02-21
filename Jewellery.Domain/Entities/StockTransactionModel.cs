using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class StockTransactionModel
    {
        public int StockId { get; set; } = 0;
        public int ProductId { get; set; } = 0;
        public int TransactionType { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public decimal Weight { get; set; } = 0;
        public int ReferenceType { get; set; } = 0;
        public string ReferenceNo { get; set; } = "";
        public string TransactionDate { get; set; } = "";
        public int TypeId { get; set; } = 0;
    }
}
