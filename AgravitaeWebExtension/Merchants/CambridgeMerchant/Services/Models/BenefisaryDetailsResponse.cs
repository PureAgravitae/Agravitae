using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class BenefisaryDetailsResponse
    {
        public string error { get; set; }
        public string benefisaryId { get; set; }
        public string beneContactName { get; set; }
        public string destinationCountry { get; set; }
        public string accountNumber { get; set; }
        public string routingCode { get; set; }
        public string bankCurrency { get; set; }
        public string beneClassification { get; set; }
        public string beneCountry { get; set; }
        public string beneRegion { get; set; }
        public string beneAddress1 { get; set; }
        public string beneAddress2 { get; set; }
        public string beneCity { get; set; }
        public string benePostal { get; set; }
        //public string beneficiaryPhoneNumber { get; set; }
        public string beneEmail { get; set; }
        public string bankName { get; set; }
        public string bankCountry { get; set; }
        public string bankRegion { get; set; }
        public string bankCity { get; set; }
        public string bankPostal { get; set; }
        public string paymentReference { get; set; }
        public string iban { get; set; }
        public string beneCountryName { get; set; }
        public string accountHolderCou { get; set; }
        public string bankCountryName { get; set; }
        public string bankCurrencyDesc { get; set; }
        public string accountHolderCountryName { get; set; }
        public string swiftBicCode { get; set; }
        public bool sendPayTracker { get; set; }
        public string bankAddressLine1 { get; set; }
        public string bankAddressLine2 { get; set; }
        public string regulatoryFields { get; set; }
        public string preferredMethod { get; set; }
        public string paymentMethods { get; set; }
        public string BeneIdentifier { get; set; }
        public string DestinationCountry { get; set; }
        public string BeneficiaryPhoneNumber { get; set; }
        public bool SendPayTracker { get; set; }
        public string PurposeOfPayment { get; set; }
        public object Regulatory { get; set; }
        public string BeneCountryName { get; set; }
        public string localAccountNumber { get; set; }
    }
}
