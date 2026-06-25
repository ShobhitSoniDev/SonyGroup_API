using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class ChangePasswordModel
    {
        public string CurrentPasswordHash { get; set; } = "";
        public string NewPasswordHash { get; set; } = "";
        public int TypeId { get; set; }
    }
}
