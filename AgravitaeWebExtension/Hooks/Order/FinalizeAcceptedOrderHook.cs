using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.Order
{
    public class FinalizeAcceptedOrderHook : IHook<FinalizeAcceptedOrderHookRequest, FinalizeAcceptedOrderHookResponse>
    {
        private readonly IOrderService _orderService;
        private readonly IZLOrderZiplingoService _ziplingoSerivice;
        private readonly IZLAssociateService _ziplingoAssociateSerivice;


        public FinalizeAcceptedOrderHook(IOrderService orderService, IZLOrderZiplingoService ziplingoSerivice, IZLAssociateService ziplingoAssociateSerivice)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _ziplingoSerivice = ziplingoSerivice ?? throw new ArgumentNullException(nameof(ziplingoSerivice));
            _ziplingoAssociateSerivice = ziplingoAssociateSerivice ?? throw new ArgumentNullException(nameof(ziplingoAssociateSerivice));



        }
        public async Task<FinalizeAcceptedOrderHookResponse> Invoke(FinalizeAcceptedOrderHookRequest request, Func<FinalizeAcceptedOrderHookRequest, Task<FinalizeAcceptedOrderHookResponse>> func)
        {
                var result = await func(request);
            try
            {
                DirectScale.Disco.Extension.Order order = await _orderService.GetOrderByOrderNumber(request.Order.OrderNumber);

                if (order.OrderType == OrderType.Enrollment)
                {
                     await _ziplingoAssociateSerivice.CreateEnrollContact(order);
                }
                if (order.Status == OrderStatus.Paid || order.IsPaid)
                {
                    var totalOrders = _orderService.GetOrdersByAssociateId(request.Order.AssociateId, "").Result;
                    if (totalOrders.Length == 1)
                    {
                       await _ziplingoSerivice.CallOrderZiplingoEngagement(order, "FirstOrderCreated", false);
                       await _ziplingoSerivice.CallOrderZiplingoEngagement(order, "OrderCreated", false);
                    }
                    else
                    {
                       await _ziplingoSerivice.CallOrderZiplingoEngagement(order, "OrderCreated", false);
                    }
                }
                if (order.OrderType == OrderType.Autoship && (order.Status == OrderStatus.Declined || order.Status == OrderStatus.FraudRejected))
                {
                    await _ziplingoSerivice.CallOrderZiplingoEngagement(order, "AutoShipFailed", true);
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
