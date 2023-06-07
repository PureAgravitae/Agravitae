using DirectScale.Disco.Extension.Hooks.Commissions;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using AgravitaeWebExtension.Repositories;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Hooks
{
    public class LogRealtimeRankAdvanceHook : IHook<LogRealtimeRankAdvanceHookRequest, LogRealtimeRankAdvanceHookResponse>
    {
        private readonly IZLAssociateService _zlassociateService;
        private readonly ICustomLogRepository _customLogRepository;
        private readonly IAssociateService _associateService;
        private readonly IZLOrderZiplingoService _zlorderziplingoService;



        public LogRealtimeRankAdvanceHook(IAssociateService associateService, IZLAssociateService zlassociateService, ICustomLogRepository customLogRepository, IZLOrderZiplingoService zlorderziplingoService)
        {
            _zlassociateService = zlassociateService ?? throw new ArgumentNullException(nameof(zlassociateService));
            _customLogRepository = customLogRepository ?? throw new ArgumentNullException(nameof(customLogRepository));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _zlorderziplingoService = zlorderziplingoService ?? throw new ArgumentNullException(nameof(zlorderziplingoService));

        }
        public async Task<LogRealtimeRankAdvanceHookResponse> Invoke(LogRealtimeRankAdvanceHookRequest request, Func<LogRealtimeRankAdvanceHookRequest, Task<LogRealtimeRankAdvanceHookResponse>> func)
        {
            var result = await func(request);
            var associate = await _associateService.GetAssociate(request.AssociateId);
            try
            {
                _zlorderziplingoService.LogRealtimeRankAdvanceEvent(request);
                await _zlassociateService.UpdateContact(associate);
            }
            catch (Exception ex)
            {
                _customLogRepository.CustomErrorLog(request.OldRank, request.NewRank, "", "Error : " + ex.Message);
            }
            return result;
        }
    }
}
