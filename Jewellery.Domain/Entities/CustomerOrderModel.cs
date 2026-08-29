using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jewellery.Domain.Entities
{
    public class CustomerOrderModel
    {
        public class Order
        {
            public int OrderId { get; set; }
            public int CustomerId { get; set; }
            public int AddressId { get; set; }
            public string PaymentMode { get; set; } = "";      // COD, CARD, UPI, NETBANKING, WALLET
            public string PaymentStatus { get; set; } = "";     // PENDING, PAID, FAILED, COD_PENDING
            public string OrderStatus { get; set; } = "";       // PLACED, CONFIRMED, PROCESSING, CANCELLED
            public decimal SubTotal { get; set; }
            public decimal GstAmount { get; set; }
            public decimal ShippingCharge { get; set; }
            public decimal TotalAmount { get; set; }
            public string? RazorpayOrderId { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? UpdatedDate { get; set; }
        }

        public class OrderItem
        {
            public int OrderItemId { get; set; }
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; } = "";
            public int Quantity { get; set; }
            public decimal CurrentRate { get; set; }
            public decimal NetWeight { get; set; }
            public decimal MakingCharge { get; set; }
            public string MakingChargeType { get; set; } = "Percentage";
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }
        }

        public class OrderPayment
        {
            public int PaymentId { get; set; }
            public int OrderId { get; set; }
            public string RazorpayOrderId { get; set; } = "";
            public string? RazorpayPaymentId { get; set; }
            public string? RazorpaySignature { get; set; }
            public decimal Amount { get; set; }
            public string Currency { get; set; } = "INR";
            public string Status { get; set; } = "CREATED";     // CREATED, VERIFIED, FAILED
            public DateTime? VerifiedDate { get; set; }
            public DateTime CreatedDate { get; set; }
        }


        public class OrderPlaceRequest
        {
            public int TypeId { get; set; }        // 1 = COD, 2 = Online
            public int AddressId { get; set; }
            public string PaymentMode { get; set; } = "";
        }

        public class RazorpayOrderUpdateRequest
        {
            public int OrderId { get; set; }
            public string RazorpayOrderId { get; set; } = "";
        }

        public class PaymentVerifyRequest
        {
            public int OrderId { get; set; }
            public string RazorpayOrderId { get; set; } = "";
            public string RazorpayPaymentId { get; set; } = "";
            public string RazorpaySignature { get; set; } = "";
            public bool IsValid { get; set; }
        }
        public class OrderManageRequest
        {
            public int TypeId { get; set; }        // 1 = Get single order + items, 2 = List all orders for customer
            public int? OrderId { get; set; }       // required for TypeId 1
        }
        // Common model — public class, kisi bhi assembly se accessible
        public class OrderManageResult
        {
            public int Code { get; set; }
            public string Message { get; set; }
            public dynamic Order { get; set; }          // single row, Dapper se
            public IEnumerable<dynamic> Items { get; set; }
            public IEnumerable<dynamic> Data { get; set; }
        }
    }
}
