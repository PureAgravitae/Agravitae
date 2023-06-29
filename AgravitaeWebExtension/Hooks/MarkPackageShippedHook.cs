using DirectScale.Disco.Extension.Hooks.Orders.Packages;
using DirectScale.Disco.Extension.Hooks;

using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks
{
    public class MarkPackageShippedHook : IHook<MarkPackagesShippedHookRequest, MarkPackagesShippedHookResponse>
    {
        // private readonly IZiplingoEngagementService _ziplingoEngagementService;
        private readonly IZLOrderZiplingoService _zloderZiplingoService;

        public MarkPackageShippedHook(IZLOrderZiplingoService zloderZiplingoService)
        {
            _zloderZiplingoService = zloderZiplingoService ?? throw new ArgumentNullException(nameof(zloderZiplingoService));
        }
        public async Task<MarkPackagesShippedHookResponse> Invoke(MarkPackagesShippedHookRequest request, Func<MarkPackagesShippedHookRequest, Task<MarkPackagesShippedHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                foreach (var shipInfo in request.PackageStatusUpdates)
                {
                   await _zloderZiplingoService.SendOrderShippedEmail(shipInfo.PackageId, shipInfo.TrackingNumber);
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
