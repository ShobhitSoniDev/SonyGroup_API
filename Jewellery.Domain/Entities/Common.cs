using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class ResponseModel 
    {
        public int Code { get; set; }
        public Object Data { get; set; }
        public string Message { get; set; } = "Success";
    }
}
