using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgravitaeWebExtension.Merchants.CambridgeMerchant.Models
{
    public class CreateBenefisaryRequest
    {
        public string AccountHolderName { get; set; }
        public string AccountHolderEmail { get; set; }
        public string AssociateId { get; set; }
        public string DestinationCountry { get; set; }
        public string BankCurrency { get; set; }
        public string Classification { get; set; }
        public string[] PaymentMethods { get; set; }
        public string PreferredMethod { get; set; }
        public string LocalAccountNumber { get; set; }
        public string LocalRoutingCode { get; set; }
        public string AccountHolderRegion { get; set; }
        public string AccountHolderPostal { get; set; }
        public string BankRegion { get; set; }
        public string BankPostal { get; set; }
        public string AccountNumber { get; set; }
        public string AccountHolderAddress1 { get; set; }
        public string AccountHolderAddress2 { get; set; }
        public string AccountHolderCity { get; set; }
        public string AccountHolderCountry { get; set; }
        public string AccountHolderPhone { get; set; }
        public bool SendPayTracker { get; set; } = true;
        public string BankName { get; set; }
        public string BankRoutingCode { get; set; }
        public string SwiftBicCode { get; set; }
        public string BankAddress1 { get; set; }
        public string BankAddress2 { get; set; }
        public string BankCity { get; set; }
        public string BankCountry { get; set; }
        public RegulatoryInfo[] RegulatoryInfo { get; set; }
    }

    public class RegulatoryInfo
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
