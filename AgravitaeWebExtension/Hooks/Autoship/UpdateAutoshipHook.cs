using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.Autoship
{
    public class UpdateAutoshipHook : IHook<UpdateAutoshipHookRequest, UpdateAutoshipHookResponse>
    {
        private readonly IAutoshipService _autoshipService;
        private readonly IAssociateService _associateService;
        private readonly IZLOrderZiplingoService _zlorderziplingoService;
        private readonly IZLAssociateService _zlassociateService;


        public UpdateAutoshipHook(IAutoshipService autoshipService, IAssociateService associateService, IZLOrderZiplingoService zlorderziplingoservice, IZLAssociateService zlassociateService)
        {
            _zlorderziplingoService = zlorderziplingoservice;
            _autoshipService = autoshipService;
            _associateService = associateService;
            _zlassociateService = zlassociateService;
        }
        public async Task<UpdateAutoshipHookResponse> Invoke(UpdateAutoshipHookRequest request, Func<UpdateAutoshipHookRequest, Task<UpdateAutoshipHookResponse>> func)
        {
            UpdateAutoshipHookResponse result = await func(request);

            try
            {
                var updatedAutoshipInfo = await _autoshipService.GetAutoship(request.AutoshipInfo.AutoshipId);

                 await _zlorderziplingoService.UpdateAutoship(updatedAutoshipInfo);
                var associateSummary = await _associateService.GetAssociate(request.AutoshipInfo.AssociateId);
                await _zlassociateService.UpdateContact(associateSummary);
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
