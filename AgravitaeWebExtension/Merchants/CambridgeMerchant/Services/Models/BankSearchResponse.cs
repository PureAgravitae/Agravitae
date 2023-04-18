using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class BankSearchResponse
    {
        public string bankName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string nationalBankCode { get; set; }
        public string swiftBIC { get; set; }
        public string branchName { get; set; }
        public string error { get; set; }
    }
}
