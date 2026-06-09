using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class CustomerMasterModel
    {
        public string CustomerCode { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public string MobileNo { get; set; } = "";
        public string Email { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public int Pincode { get; set; } = 0;
        public int TypeId { get; set; } = 0;
    }
}
