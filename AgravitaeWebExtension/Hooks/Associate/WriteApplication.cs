using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Associates.Enrollment;
using System;
using System.Threading.Tasks;
using DirectScale.Disco.Extension;
using ZiplingoEngagement.Services.Interface;
using RPMSEwallet.Services.Interface;
using RPMSEwallet.Models;
using DirectScale.Disco.Extension.Services;

namespace AgravitaeWebExtension.Hooks.Associate
{
    public class WriteApplication : IHook<WriteApplicationHookRequest, WriteApplicationHookResponse>
    {
        
        private readonly IZLAssociateService _zlassociateService;
        private readonly IEwalletService _ewalletService;
        private readonly IAssociateService _associateService;


        public WriteApplication(IZLAssociateService zlassociateService, IEwalletService ewalletService, IAssociateService associateService)
        {
            _zlassociateService = zlassociateService ?? throw new ArgumentNullException(nameof(zlassociateService));
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
        }

        public Task<WriteApplicationHookResponse> Invoke(WriteApplicationHookRequest request, Func<WriteApplicationHookRequest, Task<WriteApplicationHookResponse>> func)
        {
            var response = func(request);

            // Sync User with in ewallet -------------------------

            int newUserID = response.Result.ApplicationResponse.AssociateId;

            var asssociateresponse = _associateService.GetAssociate(response.Result.ApplicationResponse.AssociateId).Result;
            _ewalletService.CreateCustomer(asssociateresponse, Convert.ToInt32(newUserID));


            var provisionReq = new SetActiveCommissionMerchantRequest { AssociateId = Convert.ToInt32(newUserID), MerchantId = 9012 };
            _ewalletService.SetActiveCommissionMerchant(provisionReq);

            // Sync User with in ewallet --------------------------

            try
            {
                _zlassociateService.CreateContact(request.Application, response.Result.ApplicationResponse);
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}