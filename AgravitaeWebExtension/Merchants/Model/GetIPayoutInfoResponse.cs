using System.Collections.Generic;

namespace WebExtension.Merchants.Models
{
    public class GetIPayoutInfoResponse
    {
        public List<IPayoutInfo> AssociateIPayoutInfos { get; set; }
        public List<FailedCommissionMerchantInfoRetrieval> FailedRetrievals { get; set; }
    }
}
