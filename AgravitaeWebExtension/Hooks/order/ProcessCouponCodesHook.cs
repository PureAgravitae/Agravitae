using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension;

namespace AgravitaeWebExtension.Hooks.order
{
    public class ProcessCouponCodesHook : IHook<ProcessCouponCodesHookRequest, ProcessCouponCodesHookResponse>
    {             
        public const string AUTOSHIP_COUPON_NAME = "10% OFF";
        private readonly ILogger<ProcessCouponCodesHook> _logger;
        public ProcessCouponCodesHook(ILogger<ProcessCouponCodesHook> logger)
        {           
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProcessCouponCodesHookResponse> Invoke(ProcessCouponCodesHookRequest request, Func<ProcessCouponCodesHookRequest, Task<ProcessCouponCodesHookResponse>> func)
        {
            var response = await func(request);
            try
            {
                if (request.OrderType == OrderType.Autoship)
                {
                    var totalQV = request.LineItems.Sum(x => (x.QV * x.Quantity));
                    if (totalQV >= 150)
                    {
                        //add 10% discount to order
                        var subTotal = request.SubTotal;
                        double discount = (double)subTotal / 100 * 10;
                        var usedCoupons = response.OrderCoupons.UsedCoupons?.ToList() ?? new List<OrderCoupon>();
                        discount = Math.Round(discount, 2);

                        if (discount > 0 && subTotal > discount)
                        {
                            usedCoupons.Add(
                                new OrderCoupon(
                                    new Coupon
                                    {
                                        Code = AUTOSHIP_COUPON_NAME,
                                        Discount = discount,
                                        CouponType = CouponType.OrderDiscount
                                    })
                                {
                                    DiscountAmount = discount
                                });


                            response.OrderCoupons.UsedCoupons = usedCoupons.ToArray();
                            response.OrderCoupons.DiscountTotal += discount;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProcessCouponCodesHookRequest: Error applying 10% discount to order for associate {request.AssociateId} - {ex.Message}");
            }
            return response;
        }

    }
}
