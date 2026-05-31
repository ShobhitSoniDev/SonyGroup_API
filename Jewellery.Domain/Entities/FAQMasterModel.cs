using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class FAQMasterModel
    {
        public int Id { get; set; } = 0;

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public string Keywords { get; set; } = string.Empty;

        public string SearchText { get; set; } = string.Empty;

        public int TypeId { get; set; } = 0;
    }
}
