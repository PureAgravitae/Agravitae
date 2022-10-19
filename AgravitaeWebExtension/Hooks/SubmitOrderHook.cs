using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using AgravitaeWebExtension.Services;

namespace AgravitaeWebExtension.Hooks
{
    public class SubmitOrderHook : IHook<SubmitOrderHookRequest, SubmitOrderHookResponse>
    {
        private readonly IAVOrderService _orderService;
        private readonly ILogger<SubmitOrderHook> _logger;

        public SubmitOrderHook(IAVOrderService orderService, ILogger<SubmitOrderHook> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<SubmitOrderHookResponse> Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, Task<SubmitOrderHookResponse>> func)
        {
            try
            {
                if (request.Order.LineItems.Length > 0)
                {
                    bool addProp65 = false;
                    foreach (var item in request.Order.LineItems)
                    {
                        var customFields = _orderService.GetItemCustomFields(item.ItemId).Result;
                        addProp65 = customFields.Select(x => x.Field1.Equals("Prop65")).FirstOrDefault();
                        if (addProp65)
                            break;

                    }
                    if (addProp65)
                        request.Order.LineItems = await _orderService.AddAdditionalItems(request.Order);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"AddAdditionalItems Failed: {ex.Message}");
            }
            var response = await func(request);
            return response;
        }
    }
}
