using AgravitaeWebExtension.Merchants.CambridgeMerchant.Models;
using AgravitaeWebExtension.Merchants.CambridgeMerchant.Services;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Merchants.EwalletMerchant.Models;
using Microsoft.AspNetCore.Mvc;
using System;


namespace AgravitaeWebExtension.Controllers.Ewallet
{
    [Route("api/[controller]")]
    [ApiController]
    public class CambridgeController : ControllerBase
    {
        private readonly ICambridgeService _cambridgeService;

        public CambridgeController(ICambridgeService cambridgeService)
        {
            _cambridgeService = cambridgeService ?? throw new ArgumentNullException(nameof(cambridgeService));
        }

        [HttpPost]
        [Route("BankSearch")]
        public IActionResult BankSearch(BankSearchRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.BankSearch(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateBenefisary")]
        public IActionResult CreateBenefisary(CreateBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.CreateBenefisary(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("DeleteBenefisary")]
        public IActionResult DeleteBenefisary(GetBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.DeleteBenefisary(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("EditBenefisary")]
        public IActionResult EditBenefisary(CreateBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.EditBenefisary(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAccessToken")]
        public IActionResult GetAccessToken(GetBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetApiToken());
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetAssociatePaymentOrder")]
        public IActionResult GetAssociatePaymentOrder(GetAssociatePaymentOrderRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetAssociatePaymentOrder(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetBenefisary")]
        public IActionResult GetBenefisary(GetBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetBenefisary(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetBenefisaryDetails")]
        public IActionResult GetBenefisaryDetails(GetBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetBenefisaryDetails(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetBenefisaryRules")]
        public IActionResult GetBenefisaryRules(GetBenefisaryRulesRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetBenefisaryRules(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCities")]
        public IActionResult GetCities(GetCitiesRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetCities(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCountries")]
        public IActionResult GetCountries()
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetCountries());
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetCurrencies")]
        public IActionResult GetCurrencies()
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetCurrencies());
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPaymentOrder")]
        public IActionResult GetPaymentOrder(GetPaymentOrderRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetPaymentOrder(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetPaymentRateInfo")]
        public IActionResult GetPaymentRateInfo(GetPaymentRateInfoRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetPamentRateInfo(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetRegions")]
        public IActionResult GetRegions(GetRegionsRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.GetRegions(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("IBANValidation")]
        public IActionResult IBANValidation(IBANValidationRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.IBANValidation(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("MoneyOutPaymentTransferToCambridge")]
        public IActionResult MoneyOutPaymentTransferToCambridge(MoneyOutPaymentTransferRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.MoneyOutPaymentTransferToCambridge(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("SearchBenefisary")]
        public IActionResult SearchBenefisary(SearchBenefisaryRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.SearchBenefisary(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("SetAssociateCustomMerchant")]
        public IActionResult SetAssociateCustomMerchant(SetActiveCommissionMerchantRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.SetActiveCommissionMerchant(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("BenefisarySetDefault")]
        public IActionResult BenefisarySetDefault(BenefisarySetDefaultRequest rObject)
        {
            try
            {
                return new Responses().OkResult(_cambridgeService.BenefisarySetDefault(rObject));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

    }
}
