using Microsoft.AspNetCore.Mvc;
using RPMSEwallet.Services.Interface;

namespace AgravitaeWebExtension.Controllers
{
    public class CustomPageController : Controller
    {
        private readonly IEwalletService _ewalletService;

        public CustomPageController(IEwalletService ewalletService)
        {
            _ewalletService = ewalletService;
        }

        public IActionResult EWalletSettings()
        {
            var ewalletSetting = _ewalletService.GetEwalletSettings();
            ViewBag.Message = ewalletSetting;
            return View();
        }
    }
}
