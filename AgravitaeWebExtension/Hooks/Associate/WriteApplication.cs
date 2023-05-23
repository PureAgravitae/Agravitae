using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Associates.Enrollment;
using System;
using System.Threading.Tasks;
using WebExtension.Merchants.EwalletMerchant.Ewallet;
using WebExtension.Merchants.EwalletMerchant.Models;
using WebExtension.Services.ZiplingoEngagementService;
using DirectScale.Disco.Extension;
using ZiplingoEngagement.Services.Interface;

namespace WebExtension.Hooks.Associate
{
    public class WriteApplication : IHook<WriteApplicationHookRequest, WriteApplicationHookResponse>
    {
        private readonly IZiplingoEngagementService _ziplingoEngagementService;
        private readonly IEwalletService _ewalletService;
        private readonly IAssociateService _zlassociateservice;



        public WriteApplication(IZiplingoEngagementService ziplingoEngagementService, IEwalletService ewalletService, IAssociateService zlassociateservice)
        {
            _ziplingoEngagementService = ziplingoEngagementService ?? throw new ArgumentNullException(nameof(ziplingoEngagementService));
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
        }

        public Task<WriteApplicationHookResponse> Invoke(WriteApplicationHookRequest request, Func<WriteApplicationHookRequest, Task<WriteApplicationHookResponse>> func)
        {
            var response = func(request);

            // Sync User with in ewallet -------------------------

            int newUserID = response.Result.ApplicationResponse.AssociateId;
            var app = new Application
            {
                FirstName = request.Application.FirstName,
                LastName = request.Application.LastName,
                ExternalId = Convert.ToString(newUserID),
                BackOfficeId = response.Result.ApplicationResponse.BackOfficeId,
                EmailAddress = request.Application.EmailAddress,
                PrimaryPhone = request.Application.PrimaryPhone,
                LanguageCode = request.Application.LanguageCode,
                BirthDate = request.Application.BirthDate,
                ApplicantAddress = request.Application.ApplicantAddress
            };
            _ewalletService.CreateCustomer(app, Convert.ToInt32(newUserID));


            var provisionReq = new SetActiveCommissionMerchantRequest { AssociateId = Convert.ToInt32(newUserID), MerchantId = 9012 };
            _ewalletService.SetActiveCommissionMerchant(provisionReq);

            // Sync User with in ewallet --------------------------

            try
            {
                _ziplingoEngagementService.CreateContact(request.Application, response.Result.ApplicationResponse);
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