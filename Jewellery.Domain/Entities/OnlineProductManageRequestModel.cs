using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class OnlineProductManageRequestModel
    {
        public int TypeId { get; set; }
        public int ProductId { get; set; }

        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }

        public bool? IsFeatured { get; set; }
        public bool? ShowOnWeb { get; set; }
    }
    public class ProductImagesManageModel
    {
        public int TypeId { get; set; }

        public int? ProductId { get; set; }

        public int? ImageId { get; set; }

        public bool? IsPrimary { get; set; }

        public int? DisplayOrder { get; set; }

        public string? ImagePath { get; set; }
    }
    public class ProductImageModel 
    { 
        public int? ImageId { get; set; }
        public string? ImagePath { get; set; } 
        public bool IsPrimary { get; set; } 
        public int DisplayOrder { get; set; } 
    }
}
