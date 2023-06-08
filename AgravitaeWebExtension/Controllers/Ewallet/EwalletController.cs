using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgravitaeWebExtension.Models;
using AgravitaeWebExtension.Merchants.Models;
using AgravitaeWebExtension.Merchants;
using AgravitaeWebExtension.Helper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Newtonsoft.Json.Linq;
using RPMSEwallet.Services.Interface;
using RPMSEwallet.Models;

namespace AgravitaeWebExtension.Controllers.Ewallet
{
    [Route("api/[controller]")]
    [ApiController]
    public class EwalletController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IMoneyOutService _moneyOutService;
        private readonly IEwalletService _ewalletService;
        public EwalletController(IClientService clientService, IEwalletService ewalletService, IMoneyOutService moneyOutService)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _moneyOutService = moneyOutService ?? throw new ArgumentNullException(nameof(moneyOutService));
            _ewalletService = ewalletService ?? throw new ArgumentNullException(nameof(ewalletService));
        }

        [HttpPost]
        [Route("GetAssociateMerchantAccountInfo")]
        public IActionResult GetAssociateMerchantAccountInfo(GetAssociateMerchantAccountInfoRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_clientService.GetAssociateMerchantAccountInfo(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCommissionMerchantInfo")]
        public IActionResult GetCommissionMerchantInfo(GetCommissionMerchantInfoRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_clientService.GetCommissionMerchantInfo(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPointBalance")]
        public async Task<IActionResult> GetPointBalance(GetPointBalanceRequest rObject)
        {
            try
            {
              var result = await  _ewalletService.GetPointBalance(rObject);
                return new Responses().OkResult(result);
            }
            catch (Exception e)
            {
                return new Responses().BadRequestResult(e.Message);
            }
        }

        [HttpPost]
        [Route("SaveACHAccountInfo")]
        public IActionResult SaveACHAccountInfo(OnFileMerchant rObject)
        {
            try
            {
                OnFileMerchant[] currentMerchantPref;
                currentMerchantPref = _moneyOutService.GetOnFileMerchants(rObject.AssociateId).Result;
                int activeMerchantId = _clientService.GetAssociateActiveCommissionMerchant(rObject.AssociateId);

                // This adds the custom values to the appropriate table but also changes the default commission merchant
                _moneyOutService.SetActiveOnFileMerchant(rObject);

                if (currentMerchantPref != null && currentMerchantPref.Any() && activeMerchantId != 0 && currentMerchantPref.Select(x => x.MerchantId).Contains(activeMerchantId))
                {
                    var activeMerchant = currentMerchantPref.FirstOrDefault(x => x.MerchantId == activeMerchantId);

                    // Set default merchant back to what it was
                    _moneyOutService.SetActiveOnFileMerchant(new OnFileMerchant
                    {
                        AssociateId = rObject.AssociateId,
                        MerchantName = activeMerchant.MerchantName,
                        MerchantId = activeMerchant.MerchantId,
                        CustomValues = activeMerchant.CustomValues
                    });
                }
            }
            catch (Exception e)
            {
                return new Responses().BadRequestResult(e.Message);
            }

            return new Responses().OkResult(rObject);
        }

        [HttpPost]
        [Route("UpdateEwalletSettings")]
        public IActionResult UpdateEwalletSettings(RPMSEwallet.Models.EwalletSettingsRequest rObject)
        {
            try
            {
                _ewalletService.UpdateEwalletSettings(rObject);
                return new Responses().OkResult("1");
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSavePaymentFrameDetails")]
        public IActionResult GetSavePaymentFrameDetails(GetAssociateMerchantAccountInfoRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_clientService.GetAssociateMerchantAccountInfo(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
