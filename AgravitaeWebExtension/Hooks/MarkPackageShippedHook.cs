using DirectScale.Disco.Extension.Hooks.Orders.Packages;
using DirectScale.Disco.Extension.Hooks;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks
{
    public class MarkPackageShippedHook : IHook<MarkPackagesShippedHookRequest, MarkPackagesShippedHookResponse>
    {
        private readonly IZLOrderZiplingoService _zlorderziplingoService;

        public MarkPackageShippedHook(IZLOrderZiplingoService zlorderziplingoService)
        {
            _zlorderziplingoService = zlorderziplingoService ?? throw new ArgumentNullException(nameof(zlorderziplingoService));
        }
        public async Task<MarkPackagesShippedHookResponse> Invoke(MarkPackagesShippedHookRequest request, Func<MarkPackagesShippedHookRequest, Task<MarkPackagesShippedHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                foreach (var shipInfo in request.PackageStatusUpdates)
                {
                     await _zlorderziplingoService.SendOrderShippedEmail(shipInfo.PackageId, shipInfo.TrackingNumber);
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
