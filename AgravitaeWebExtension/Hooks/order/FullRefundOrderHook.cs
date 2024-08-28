using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using ZiplingoEngagement.Services.Interface;
using AgravitaeWebExtension.Services;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FullRefundOrderHook : IHook<FullRefundOrderHookRequest, FullRefundOrderHookResponse>
    {

        private IZLOrderZiplingoService _zloderZiplingoService;
        // public IZiplingoEngagementService _ziplingoEngagementService;
        private readonly ICustomLogService _customservice;

        public FullRefundOrderHook(IZLOrderZiplingoService zloderZiplingoService, ICustomLogService customservice)
        {
            _zloderZiplingoService = zloderZiplingoService ?? throw new ArgumentNullException(nameof(zloderZiplingoService));
            _customservice = customservice;
        }

        public async Task<FullRefundOrderHookResponse> Invoke(FullRefundOrderHookRequest request, Func<FullRefundOrderHookRequest, Task<FullRefundOrderHookResponse>> func)
        {
            try
            {
                var response = await func(request);
                _customservice.SaveLog(0, request.Order.OrderNumber, "FullRefund Order Hook Called", "Test FullRefundOrderHook", "", "", "", "", "");

                _zloderZiplingoService.CallFullRefundOrderZiplingoEngagementTrigger(request.Order, "FullRefundOrder", false);
                return response;
            }
            catch (Exception e)
            {

            }
            return new FullRefundOrderHookResponse();
        }
    }
}
