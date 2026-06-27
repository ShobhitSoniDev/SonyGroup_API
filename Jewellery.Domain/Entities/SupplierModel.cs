using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class SupplierModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Phone { get; set; }
        public string GSTIN { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public int TypeId { get; set; }
    }
}
