﻿using DirectScale.Disco.Extension.Hooks.Autoships;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using AgravitaeWebExtension.Services.ZiplingoEngagementService;

namespace AgravitaeWebExtension.Hooks.Autoship
{
    public class UpdateAutoshipHook : IHook<UpdateAutoshipHookRequest, UpdateAutoshipHookResponse>
    {
        private readonly IZiplingoEngagementService _ziplingoEngagementService;
        private readonly IAutoshipService _autoshipService;
        private readonly IAssociateService _associateService;

        public UpdateAutoshipHook(IZiplingoEngagementService ziplingoEngagementService, IAutoshipService autoshipService, IAssociateService associateService)
        {
            _ziplingoEngagementService = ziplingoEngagementService;
            _autoshipService = autoshipService;
            _associateService = associateService;
        }
        public async Task<UpdateAutoshipHookResponse> Invoke(UpdateAutoshipHookRequest request, Func<UpdateAutoshipHookRequest, Task<UpdateAutoshipHookResponse>> func)
        {
            UpdateAutoshipHookResponse result = await func(request);

            try
            {
                var updatedAutoshipInfo = await _autoshipService.GetAutoship(request.AutoshipInfo.AutoshipId);
                _ziplingoEngagementService.UpdateAutoshipTrigger(updatedAutoshipInfo);
                var associateSummary = await _associateService.GetAssociate(request.AutoshipInfo.AssociateId);
                _ziplingoEngagementService.UpdateContact(associateSummary);
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}