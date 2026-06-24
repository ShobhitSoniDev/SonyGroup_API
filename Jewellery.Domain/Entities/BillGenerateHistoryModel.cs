using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class BillGenerateHistoryModel
    {
        public int TypeId { get; set; }

        public long? BillGenerateId { get; set; }

        public string CustomerCode { get; set; }

        public string? BillNo { get; set; }

        public string? FilePath { get; set; }

        public string? Description { get; set; }

        public int? LanguageType { get; set; }
    }
}
