using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebExtension.Merchants.EwalletMerchant.Models
{

    public class CustomerPointTransactionsRequest
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public string ExternalCustomerID { get; set; }
        public string PointAccountID { get; set; }
        public TransactionType TransactionType { get; set; }
        public RedeemType RedeemType { get; set; }
        public string ReferenceNo { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }

    public enum RedeemType
    {
        Initial,
        Order,
        Autoship,
        SignupFee,
        Transfer,
        Adjust,
        Other,
        IPayoutTransferfee,
        ACHTransferfee,
        CambridgeTransferfee,
        CheckTransferfee,
        Commission
    }

    public enum TransactionType
    {
        Credit,
        Debit,
    }
}
