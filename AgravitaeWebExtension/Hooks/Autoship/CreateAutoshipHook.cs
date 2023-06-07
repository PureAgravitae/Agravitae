using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks.Autoship
{
    public class CreateAutoshipHook : IHook<CreateAutoshipHookRequest, CreateAutoshipHookResponse>
    {
        private readonly IAssociateService _associateService;
        private readonly IAutoshipService _autoshipService;
        private readonly IZLOrderZiplingoService _zlorderziplingoService;
        private readonly IZLAssociateService _zlassociateService;

        public CreateAutoshipHook(IAssociateService associateService, IAutoshipService autoshipService, IZLOrderZiplingoService zlorderziplingoservice, IZLAssociateService zlassociateService)
        {
            _associateService = associateService;
            _autoshipService = autoshipService;
            _zlorderziplingoService = zlorderziplingoservice;
            _zlassociateService = zlassociateService;
        }
        public async Task<CreateAutoshipHookResponse> Invoke(CreateAutoshipHookRequest request, Func<CreateAutoshipHookRequest, Task<CreateAutoshipHookResponse>> func)
        {
            var result = await func(request);
            try
            {
                var autoshipInfo = await _autoshipService.GetAutoship(result.AutoshipId);
                await _zlorderziplingoService.CreateAutoship(autoshipInfo);
                var associateSummary = await _associateService.GetAssociate(autoshipInfo.AssociateId);
                await _zlassociateService.UpdateContact(associateSummary);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
