using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FinalizeNonAcceptedOrder : IHook<FinalizeNonAcceptedOrderHookRequest, FinalizeNonAcceptedOrderHookResponse>
    {
        //private readonly IZiplingoEngagementService _ziplingoEngagementService;
        private readonly IZLOrderZiplingoService _zlorderService;

        public FinalizeNonAcceptedOrder(IZLOrderZiplingoService zlorderService)
        {
            _zlorderService = zlorderService ?? throw new ArgumentNullException(nameof(zlorderService));
        }

        public async Task<FinalizeNonAcceptedOrderHookResponse> Invoke(FinalizeNonAcceptedOrderHookRequest request, Func<FinalizeNonAcceptedOrderHookRequest, Task<FinalizeNonAcceptedOrderHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                if (request.Order.OrderType == OrderType.Autoship)
                {
                    await _zlorderService.CallOrderZiplingoEngagement(request.Order, "AutoShipFailed", true);
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
