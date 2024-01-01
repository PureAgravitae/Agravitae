using AgravitaeExtension.Merchants.Tyga.Interfaces;
using AgravitaeExtension.Merchants.Tyga.Models;
using AgravitaeWebExtension.Helper;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using RPMSEwallet.Services;
using RPMSEwallet.Services.Interface;
using ZiplingoEngagement.Models.Request;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IEwalletService _ewalletService;
        private readonly ITygaRepository _tygaRepository;
        private readonly IZLSettingsService _zLSettingsService;

        public TestApiController(IDataService dataService, IEwalletService ewalletService, IZLSettingsService zLSettingsService, ITygaRepository _tygaRepository)
        {
            _dataService = dataService;
            _ewalletService = ewalletService;
            _zLSettingsService = zLSettingsService;
        }

        [HttpGet]
        [Route("GetDatabaseConnection")]
        public IActionResult TestApi()
        {
            var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result);
            return Ok(dbConnection.ConnectionString);
        }
        [HttpPost]
        [Route("UpdateEwalletSetting")]
        public IActionResult UpdateEwalletSetting(RPMSEwallet.Models.EwalletSettingsRequest request)
        {
             _ewalletService.UpdateEwalletSettings(request);
            return Ok();
        }
        [HttpPost]
        [Route("UpdateZiplingoEngagementSettings")]
        public IActionResult UpdateZiplingoEngagementSettings(ZiplingoEngagementSettingsRequest request)
        {
            try
            {

                _zLSettingsService.UpdateSettings(request);
                return new Responses().OkResult();
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
        [HttpPost]
        [Route("UpdateTygaSettings")]
        public IActionResult UpdateTygaSettings(TygaSettings request)
        {
            try
            {
                _tygaRepository.UpdateTygaSettings(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


    }
}
