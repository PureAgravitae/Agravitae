using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class MoneyOutPaymentTransferResponse
    {
        public string orderPaymentNumber { get; set; }
        public double settlementAmount { get; set; }
        public double amountTotal { get; set; }
        public string error { get; set; }
    }
}
