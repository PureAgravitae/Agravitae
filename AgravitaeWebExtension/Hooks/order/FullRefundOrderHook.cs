﻿using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using AgravitaeWebExtension.Services.ZiplingoEngagementService;

namespace AgravitaeWebExtension.Hooks.order
{
    public class FullRefundOrderHook : IHook<FullRefundOrderHookRequest, FullRefundOrderHookResponse>
    {


        public IZiplingoEngagementService _ziplingoEngagementService;

        public FullRefundOrderHook(IZiplingoEngagementService ZiplingoEngagementService)
        {

            _ziplingoEngagementService = ZiplingoEngagementService ?? throw new ArgumentNullException(nameof(ZiplingoEngagementService));
        }

        public async Task<FullRefundOrderHookResponse> Invoke(FullRefundOrderHookRequest request, Func<FullRefundOrderHookRequest, Task<FullRefundOrderHookResponse>> func)
        {
            try
            {
                var response = await func(request);

                _ziplingoEngagementService.CallFullRefundOrderZiplingoEngagementTrigger(request.Order, "FullRefundOrder", false);
                return response;
            }
            catch (Exception e)
            {

            }
            return new FullRefundOrderHookResponse();
        }
    }
}
