using AgravitaeExtension.Merchants.Tyga.Interfaces;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using RPMSEwallet.Services.Interface;
using ZiplingoEngagement.Models.Request;
using ZiplingoEngagement.Models.Settings;
using ZiplingoEngagement.Services.Interface;
using static Dapper.SqlMapper;

namespace AgravitaeWebExtension.Controllers
{
    public class CustomPageController : Controller
    {
        private readonly IEwalletService _ewalletService;
        private readonly IZLSettingsService _zLSettingsService;
        private readonly ITygaRepository _tygaRepository;

        public CustomPageController(IEwalletService ewalletService, IZLSettingsService zLSettingsService, ITygaRepository tygaRepository)
        {
            _ewalletService = ewalletService;
            _zLSettingsService = zLSettingsService;
            _tygaRepository = tygaRepository;
        }

        public IActionResult EWalletSettings()
        {
            var ewalletSetting = _ewalletService.GetEwalletSettings();
            ViewBag.Message = ewalletSetting;
            return View();
        }
        public IActionResult ZiplingoEngagementSetting()
        {
            var settings = _zLSettingsService.GetSettings();
            var eventSettings =_zLSettingsService.GetEventSettingsList().Result;
            var viewDataSend = new
            {
                settings,
                eventSettings
            };
            ViewBag.Message = viewDataSend;
            return View();
        }
        public IActionResult TygaSettings()
        {
            var tygaSetting = _tygaRepository.GetTygaSettings().GetAwaiter().GetResult();
            ViewBag.Message = tygaSetting;
            return View();
        }
    }
}
