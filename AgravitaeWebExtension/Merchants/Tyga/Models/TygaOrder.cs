using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeExtension.Merchants.Tyga.Models
{
    public class TygaOrder
    {

        public string recordnumber { get; set; }
        public string AssociateID { get; set; }
        public string OrderID { get; set; }
        public string TygaOrderId { get; set; }
        public string Message { get; set; }
        public string PaymentUrl { get; set; }
        public string Status { get; set; }
        public string TransactionNumber { get; set; }
    }
}
