using System.Collections.Generic;

namespace WebExtension.Merchants.Models
{
    public class GetAchInfoResponse
    {
        public List<AchInfo> AssociateAchInfos { get; set; }
        public List<FailedCommissionMerchantInfoRetrieval> FailedRetrievals { get; set; }
    }
}
