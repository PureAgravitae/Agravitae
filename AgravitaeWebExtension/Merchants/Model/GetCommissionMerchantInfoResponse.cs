namespace AgravitaeWebExtension.Merchants.Models
{
    public class GetCommissionMerchantInfoResponse
    {
        public int BatchId { get; set; }
        public GetAchInfoResponse AchInfo { get; set; }
        public GetIPayoutInfoResponse IPayoutInfo { get; set; }
        public GetCheckInfoResponse CheckResponse { get; set; }
    }
}
