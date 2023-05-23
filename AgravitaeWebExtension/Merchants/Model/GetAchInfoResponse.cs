using System.Collections.Generic;

namespace AgravitaeWebExtension.Merchants.Models
{
    public class GetAchInfoResponse
    {
        public List<AchInfo> AssociateAchInfos { get; set; }
        public List<FailedCommissionMerchantInfoRetrieval> FailedRetrievals { get; set; }
    }
}
