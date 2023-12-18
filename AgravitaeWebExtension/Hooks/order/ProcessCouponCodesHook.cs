using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension;
using AgravitaeWebExtension.Repositories;

namespace AgravitaeWebExtension.Hooks.order
{
    public class ProcessCouponCodesHook : IHook<ProcessCouponCodesHookRequest, ProcessCouponCodesHookResponse>
    {             
        public const string AUTOSHIP_COUPON_NAME = "10% OFF";
        private readonly ILogger<ProcessCouponCodesHook> _logger;
        private readonly IOrdersInfoRepository _ordersInfoRepo;
        public ProcessCouponCodesHook(
            ILogger<ProcessCouponCodesHook> logger,
            IOrdersInfoRepository ordersInfoRepo
        ) {           
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ordersInfoRepo = ordersInfoRepo ?? throw new ArgumentNullException( nameof(ordersInfoRepo));
        }

        public async Task<ProcessCouponCodesHookResponse> Invoke(ProcessCouponCodesHookRequest request, Func<ProcessCouponCodesHookRequest, Task<ProcessCouponCodesHookResponse>> func)
        {
            var response = await func(request);
            await VerifyLifetimeSKULimit(request);
            try
            {
                if (request.OrderType == OrderType.Autoship)
                {
                    var totalQV = request.LineItems.Sum(x => (x.QV * x.Quantity));
                    if (totalQV >= 150)
                    {
                        //add 10% discount to order
                        var subTotal = request.SubTotal;
                        if (subTotal > 0)
                        {
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ProcessCouponCodesHookRequest: Error applying 10% discount to order for associate {request.AssociateId} - {ex.Message}");
            }
            return response;
        }

        Dictionary<string, int> skuLimit = new Dictionary<string, int> {
            { "14CAPAPSMPL", 2 },
            { "14CAPAPSMPL (Canada)", 2 }
        };
        public async Task VerifyLifetimeSKULimit(ProcessCouponCodesHookRequest request) {
            //first we validate if the request contains any SKU with limits
            var applicableSKUs = request.LineItems.Where(x => skuLimit.ContainsKey(x.SKU) && x.Quantity > 0);
            if (applicableSKUs.Count() == 0) return;

            //then we get the historical total per Associate per applicable SKU
            var results = await _ordersInfoRepo.GetItemOrderHistoryCountPerAssociate(request.AssociateId, applicableSKUs.Select(x => x.SKU).ToArray());

            foreach (var res in results)
            {
                if (skuLimit.TryGetValue(res.SKU, out var maxQty)) {
                    //now we add the current order items to the historical ones
                    var qtyInOrder = request.LineItems.First(x => x.SKU == res.SKU).Quantity;
                    var totalQty = res.Qty + qtyInOrder;
                    if (maxQty <= totalQty)
                        throw new Exception($"You have reached the maximum available quantity of {maxQty} (Previously ordered: {res.Qty}. Qty in this order: {qtyInOrder}) for SKU: ({res.SKU})");
                }
            }

        }

    }
}
