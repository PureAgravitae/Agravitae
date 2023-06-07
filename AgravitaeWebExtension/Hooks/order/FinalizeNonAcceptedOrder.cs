using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FinalizeNonAcceptedOrder : IHook<FinalizeNonAcceptedOrderHookRequest, FinalizeNonAcceptedOrderHookResponse>
    {
        private readonly IZLOrderZiplingoService _zlorderziplingoService;


        public FinalizeNonAcceptedOrder(IZLOrderZiplingoService zlorderziplingoService)
        {
            _zlorderziplingoService = zlorderziplingoService ?? throw new ArgumentNullException(nameof(zlorderziplingoService));
        }

        public async Task<FinalizeNonAcceptedOrderHookResponse> Invoke(FinalizeNonAcceptedOrderHookRequest request, Func<FinalizeNonAcceptedOrderHookRequest, Task<FinalizeNonAcceptedOrderHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                if (request.Order.OrderType == OrderType.Autoship)
                {
                    await _zlorderziplingoService.CallOrderZiplingoEngagement(request.Order, "AutoShipFailed", true);
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
