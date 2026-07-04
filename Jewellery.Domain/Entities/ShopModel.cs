using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class ShopModel
    {
        public int ShopId { get; set; }
        public string ShopCode { get; set; } = "";
        public string ShopName { get; set; } = "";
        public string TagLine { get; set; } = "";
        public string OwnerName { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string GSTNo { get; set; } = "";
        public string Logo { get; set; } = "";
        public bool IsActive { get; set; } = true;
        public int TypeId { get; set; }
    }
}
