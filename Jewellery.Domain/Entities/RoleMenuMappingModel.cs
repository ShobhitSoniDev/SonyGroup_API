using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class RoleMenuMappingModel
    {
        public int RoleId { get; set; }
        public string MenuIds { get; set; } = "";
        public int TypeId { get; set; }
    }
}
