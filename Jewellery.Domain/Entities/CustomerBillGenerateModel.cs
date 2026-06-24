using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class CustomerBillGenerateModel
    {
        public class ShopInfo
        {
            public string ShopName { get; set; }
            public string Phone { get; set; }

            public static ShopInfo Parse(string input)
            {
                var p = input.Split('/');
                return new ShopInfo
                {
                    ShopName = p[0].Trim(),
                    Phone = p.Length > 1 ? p[1].Trim() : string.Empty
                };
            }
        }

        public class CustomerInfo
        {
            public string Name { get; set; }
            public string Phone { get; set; }

            public static CustomerInfo Parse(string input)
            {
                var p = input.Split('/');
                return new CustomerInfo
                {
                    Name = p[0].Trim(),
                    Phone = p.Length > 1 ? p[1].Trim() : string.Empty
                };
            }
        }

        public class JewelleryItem
        {
            public string Name { get; set; }
            public double Weight { get; set; }
            public decimal Price { get; set; }
            public bool IsOld { get; set; }
        }

        public class BillInfo
        {
            public DateTime Date { get; set; } = DateTime.Today;
            public List<JewelleryItem> NewItems { get; set; } = new();
            public List<JewelleryItem> OldItems { get; set; } = new();
        }
        public class BillLabels
        {
            public string HtmlLang { get; set; }
            public string SubTitle { get; set; }   // "Fine Jewellery" etc.
            public string CustomerLabel { get; set; }
            public string BillNoLabel { get; set; }
            public string DateLabel { get; set; }
            public string NewSectionTitle { get; set; }
            public string OldSectionTitle { get; set; }
            public string ColItem { get; set; }
            public string ColWeight { get; set; }
            public string ColAmount { get; set; }
            public string SummaryNew { get; set; }
            public string SummaryOld { get; set; }
            public string SummaryNet { get; set; }
            public string FooterTagline { get; set; }
            public string PrintBtn { get; set; }
        }
    }
}
