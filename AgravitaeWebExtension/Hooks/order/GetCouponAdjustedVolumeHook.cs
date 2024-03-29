﻿using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension;

namespace AgravitaeWebExtension.Hooks.order
{
    public class GetCouponAdjustedVolumeHook : IHook<GetCouponAdjustedVolumeHookRequest, GetCouponAdjustedVolumeHookResponse>
    {
        private readonly ILogger<GetCouponAdjustedVolumeHook> _logger;
        public const string PLACEHOLDER_COUPON_NAME = "__unused__";
        public const string AUTOSHIP_COUPON_NAME = "10% OFF";
        public GetCouponAdjustedVolumeHook(ILogger<GetCouponAdjustedVolumeHook> logger)
        {            
            _logger = logger;
        }
        public async Task<GetCouponAdjustedVolumeHookResponse> Invoke(GetCouponAdjustedVolumeHookRequest request, Func<GetCouponAdjustedVolumeHookRequest, Task<GetCouponAdjustedVolumeHookResponse>> func)
        {
            try
            {
                // Add only the coupons that we WANT to reduce volume.
                var usedCoupons = new List<OrderCoupon>();
                foreach (var total in request.Totals)
                {
                    if (total.Coupons.UsedCoupons.Length > 0)
                    {
                        foreach (var coupon in total.Coupons.UsedCoupons)
                        {
                            if (!coupon.Info.Code.EndsWith(PLACEHOLDER_COUPON_NAME, StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (coupon.Info.AmountType == AmountType.Percent)
                                {
                                    usedCoupons.Add(coupon);
                                }                                
                            }
                            
                        }
                        total.Coupons.UsedCoupons = usedCoupons.ToArray();
                    }
                }
                var result = await func(request);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetCouponAdjustedVolumeHookResponse: Error in GetCouponAdjustedVolumeHook - {ex.Message}");
                return new GetCouponAdjustedVolumeHookResponse();
            }
        }

    }
}
