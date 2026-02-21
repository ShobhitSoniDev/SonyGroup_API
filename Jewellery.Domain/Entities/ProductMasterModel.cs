using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class ProductMasterModel
    {
        public int ProductId { get; set; } = 0;
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; } = 0;
        public int MetalId { get; set; } = 0;
        public decimal GrossWeight { get; set; } = 0;
        public decimal NetWeight { get; set; } = 0;
        public decimal WastageWeight { get; set; } = 0;
        public decimal MakingCharge { get; set; } = 0;
        public decimal RatePerGram { get; set; } = 0;
        public int TotalQuantity { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }
}
