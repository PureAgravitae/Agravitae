using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.EwalletMerchant.Models
{
    public class RefundTransaction
    {
        public string Id { get; set; }

        public RefundData RefundData { get; set; }
    }

    public class RefundData
    {
        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public decimal PartialAmount { get; set; }

    }
}
