using System.Collections.Generic;

namespace AgravitaeWebExtension.Merchants.Models
{
    public class GetIPayoutInfoResponse
    {
        public List<IPayoutInfo> AssociateIPayoutInfos { get; set; }
        public List<FailedCommissionMerchantInfoRetrieval> FailedRetrievals { get; set; }
    }
}
