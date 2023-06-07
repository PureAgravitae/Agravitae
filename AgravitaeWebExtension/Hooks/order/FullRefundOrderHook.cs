using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FullRefundOrderHook : IHook<FullRefundOrderHookRequest, FullRefundOrderHookResponse>
    {

        private readonly IZLOrderZiplingoService _zlorderziplingoService;


        public FullRefundOrderHook(IZLOrderZiplingoService zlorderziplingoService)
        {
            _zlorderziplingoService = zlorderziplingoService ?? throw new ArgumentNullException(nameof(zlorderziplingoService));
        }

        public async Task<FullRefundOrderHookResponse> Invoke(FullRefundOrderHookRequest request, Func<FullRefundOrderHookRequest, Task<FullRefundOrderHookResponse>> func)
        {
            try
            {
                var response = await func(request);

                 await _zlorderziplingoService.CallFullRefundOrderZiplingoEngagementTrigger(request.Order, "FullRefundOrder", false);
                return response;
            }
            catch (Exception e)
            {

            }
            return new FullRefundOrderHookResponse();
        }
    }
}
