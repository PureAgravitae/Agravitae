using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetPaymentRateInfoResponse
    {
        public string error { get; set; }
        public double InitialWithdrawlAmount { get; set; }
        public double CurrencyRate { get; set; }
        public double InternationalTransferFees { get; set; }
        public double PayoutAmount { get; set; }
        public string FeeCurrency { get; set; }
        public string PaymentCurrency { get; set; }
        public string SettlementCurrency { get; set; }
    }
}
