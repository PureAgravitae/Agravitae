using AgravitaeWebExtension.Services;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyRun_CustomAPI : ControllerBase
    {
        private readonly IZLAssociateService _zlAssociateService;
        private readonly IDailyRunCustomService _dailyrunService;
        private readonly IZLOrderZiplingoService _zloderZiplingoService;
        private readonly IAssociateWebService _associateWebService;

        public DailyRun_CustomAPI(IZLAssociateService zlAssociateService,IDailyRunCustomService dailyrunService, IZLOrderZiplingoService zloderZiplingoService, IAssociateWebService associateWebService)
        {
            _zlAssociateService = zlAssociateService ?? throw new ArgumentNullException(nameof(zlAssociateService));
            _dailyrunService = dailyrunService ?? throw new ArgumentNullException(nameof(dailyrunService));
            _zloderZiplingoService = zloderZiplingoService;
            _associateWebService = associateWebService;
        }
        [HttpPost("Custom_Dailyrun")]
        public IActionResult DailyRunCustomApi()
        {
            try
            {
                try
                {
                    var autoships = _dailyrunService.GetNextFiveDayAutoships();
                    _zlAssociateService.FiveDayRun(autoships);
                }
                catch (Exception)
                {

                }
                try
                {
                  var expiryCreditCardInfoBefore30Days = _dailyrunService.GetCreditCardInfoBefore30Days();

                    _zlAssociateService.ExpirationCard(expiryCreditCardInfoBefore30Days);
                }
                catch (Exception)
                {

                }
                try
                {
                    _zlAssociateService.AssociateBirthDay();
                    _zlAssociateService.AssociateWorkAnniversary();
                    _zlAssociateService.ExecuteCommissionEarned();
                }
                catch (Exception)
                {

                    throw;
                }
                //update ewallet associate status
                try
                {
                    _dailyrunService.UpdateAssociateStatusinUnify();
                }
                catch (Exception)
                {
                    throw;
                }
                try
                {
                    var response = _associateWebService.GetShipMethods();
                        var isUpdated = response.All(x => x.isUpdated);
                    if (isUpdated)
                    {
                        _zloderZiplingoService.UpdateShipMethods(response);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            catch (Exception ex)
            {

            }

            return Ok();
        }


    }
}
