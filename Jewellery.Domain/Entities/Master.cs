using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class MetalMasterModel
    {
        public int MetalId { get; set; }
        public string MetalName { get; set; }
        public int Purity { get; set; }
    }
    public class CategoryMasterModel
    {
        public int MetalId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
