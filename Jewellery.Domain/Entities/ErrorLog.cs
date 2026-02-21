using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string ApiName { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public int? LineNumber { get; set; }
        public DateTime CreatedDate { get; set; }
    }

}
