
namespace WebExtension.Merchants.CambridgeMerchant.Models
{
    public class MoneyOutPaymentTransferRequest
    {
        public int AssociateID { get; set; }
        public string PaymentCurrency { get; set; }
        public string SettlementCurrency { get; set; }
        public string RemitterId { get; set; }
        public int Amount { get; set; }
    }
}
