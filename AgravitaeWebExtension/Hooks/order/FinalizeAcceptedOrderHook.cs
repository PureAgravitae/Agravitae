using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Services;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FinalizeAcceptedOrderHook : IHook<FinalizeAcceptedOrderHookRequest, FinalizeAcceptedOrderHookResponse>
    {
           //private readonly IZiplingoEngagementService _ziplingoEngagementService;
           private readonly IOrderService _orderService;
           private readonly IZLAssociateService _zlassociateService;
           private readonly IZLOrderZiplingoService _zlorderService;
        public FinalizeAcceptedOrderHook(IZLAssociateService zlassociateService, IOrderService orderService, IZLOrderZiplingoService zlorderService)
        {
            _zlassociateService = zlassociateService ?? throw new ArgumentNullException(nameof(zlassociateService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _zlorderService = zlorderService ?? throw new ArgumentNullException(nameof(zlorderService));
        }
        public async Task<FinalizeAcceptedOrderHookResponse> Invoke(FinalizeAcceptedOrderHookRequest request, Func<FinalizeAcceptedOrderHookRequest, Task<FinalizeAcceptedOrderHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                DirectScale.Disco.Extension.Order order = await _orderService.GetOrderByOrderNumber(request.Order.OrderNumber);
                if (order.OrderType == OrderType.Enrollment)
                {
                    await _zlassociateService.CreateEnrollContact(order);
                }
                if (order.Status == OrderStatus.Paid || order.IsPaid)
                {
                    var totalOrders = _orderService.GetOrdersByAssociateId(request.Order.AssociateId, "").Result;
                    if (totalOrders.Length == 1)
                    {
                       await  _zlorderService.CallOrderZiplingoEngagement(order, "FirstOrderCreated", false);
                       await  _zlorderService.CallOrderZiplingoEngagement(order, "OrderCreated", false);
                    }
                    else
                    {
                       
                        await _zlorderService.CallOrderZiplingoEngagement(order, "OrderCreated", false);
                    }
                }
                if (order.OrderType == OrderType.Autoship && (order.Status == OrderStatus.Declined || order.Status == OrderStatus.FraudRejected))
                {
                    await _zlorderService.CallOrderZiplingoEngagement(order, "AutoShipFailed", true);
                }
            }
            catch (Exception ex)
            {
                //await _ziplingoEngagementService.SaveCustomLogs(request.Order.AssociateId, request.Order.OrderNumber,"", "Error : " + ex.Message);
            }
            return result;
        }
    }
}
