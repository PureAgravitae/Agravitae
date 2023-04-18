using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebExtension.Merchants.CambridgeMerchant.Services.Models
{
    public class GetPaymentOrderResponse
    {
        public string error { get; set; }
        public string amount { get; set; }
        public string payeeName { get; set; }
        public string accountType { get; set; }
        public string currency { get; set; }
        public string approvalStatus { get; set; }
        public string feeAmount { get; set; }
        public string feeCurrency { get; set; }
        public string estimateCostAmount { get; set; }
        public string estimateCostCurrency { get; set; }
        public string method { get; set; }
        public string entryDate { get; set; }
        public string ordNum { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public string buyAmount { get; set; }
        public string sellAmount { get; set; }
        public string exchange { get; set; }
        public string ourAction { get; set; }
    }
}
