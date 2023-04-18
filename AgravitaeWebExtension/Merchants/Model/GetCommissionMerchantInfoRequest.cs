using System.Collections.Generic;

namespace WebExtension.Merchants.Models
{
    public class GetCommissionMerchantInfoRequest
    {
        public int BatchId { get; set; }
        public int MerchantId { get; set; }
        public List<int> AssociateIds { get; set; }
    }
}
