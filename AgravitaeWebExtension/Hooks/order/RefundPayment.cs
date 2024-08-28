using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using AgravitaeWebExtension.Services;

namespace AgravitaeWebExtension.Hooks.order
{
    public class RefundPayment : IHook<RefundPaymentHookRequest, RefundPaymentHookResponse>
    {
        private readonly ICustomLogService _customservice;
        public RefundPayment(ICustomLogService customservice)
        {
            _customservice = customservice;
        }
        public async Task<RefundPaymentHookResponse> Invoke(RefundPaymentHookRequest request, Func<RefundPaymentHookRequest, Task<RefundPaymentHookResponse>> func)
        {
            var response = await func(request);

            _customservice.SaveLog(0, request.OrderNumber, "refund Order Hook Called", "Test RefundPaymentHook","","","","","");


            return response;
        }
    }
}
