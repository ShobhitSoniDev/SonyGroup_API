using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class LoanEntryModel
    {
        public string LoanId { get; set; }
        public string CustomerCode { get; set; }
        public string LoanType { get; set; }
        public decimal Amount { get; set; }
        public string InterestType { get; set; }
        public decimal InterestRate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Duration { get; set; }
        public string MetalType { get; set; }
        public decimal Weight { get; set; }
        public string ItemCount { get; set; }
        public string Description { get; set; }
        public string PhotoPath { get; set; }
        public string TypeId { get; set; }
    }
}
