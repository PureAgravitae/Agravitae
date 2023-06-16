using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeExtension.Merchants.Tyga.Models
{
    public class TygaPaymentResponse
    {
        
     public string TxId { get; set; }
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
    
    }
}
