using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class IBANValidationResponse
    {
        public bool isValid { get; set; }
        public string accountNumber { get; set; }
        public string branchName { get; set; }
        public string bankName { get; set; }
        public string iban { get; set; }
        public string branchCode { get; set; }
        public string routingNumber { get; set; }
        public string swiftBIC { get; set; }
        public string bankAddress { get; set; }
        public string postalCode { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
        public string bankCity { get; set; }
        public string Message { get; set; }
        public string error { get; set; }
    }
}
