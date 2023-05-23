using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Models
{
    public class GetBenefisaryRulesRequest
    {
        public string Destinationcountry { get; set; }
        public string Bankcountry { get; set; }
        public string BankCurrency { get; set; }
        public string Classification { get; set; }
        public string PaymentMethod { get; set; }
    }
}
