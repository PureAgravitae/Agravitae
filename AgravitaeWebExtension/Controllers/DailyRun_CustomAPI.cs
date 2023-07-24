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

        public DailyRun_CustomAPI(IZLAssociateService zlAssociateService,IDailyRunCustomService dailyrunService)
        {
            _zlAssociateService = zlAssociateService ?? throw new ArgumentNullException(nameof(zlAssociateService));
            _dailyrunService = dailyrunService ?? throw new ArgumentNullException(nameof(dailyrunService));
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

            }
            catch (Exception ex)
            {

            }

            return Ok();
        }


    }
}
