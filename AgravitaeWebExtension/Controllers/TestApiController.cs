
﻿using AgravitaeExtension.Merchants.Tyga.Interfaces;
using AgravitaeExtension.Merchants.Tyga.Models;
using AgravitaeWebExtension.Helper;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using RPMSEwallet.Services;
using RPMSEwallet.Services.Interface;
using ZiplingoEngagement.Models.Request;
﻿using AgravitaeWebExtension.Services;
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
        private readonly IAssociateWebService _associateWebService;
        private readonly IZLOrderZiplingoService _zloderZiplingoService;

        public TestApiController(IDataService dataService, IEwalletService ewalletService, IZLSettingsService zLSettingsService, ITygaRepository _tygaRepository, IAssociateWebService associateWebService, IZLOrderZiplingoService zloderZiplingoService)
        {
            _dataService = dataService;
            _ewalletService = ewalletService;
            _zLSettingsService = zLSettingsService;
              _associateWebService = associateWebService;
            _zloderZiplingoService = zloderZiplingoService;
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

                _zLSettingsService.UpdateSettings(request).GetAwaiter().GetResult();
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

        [Route("Test_ShipMethodSync")]
        public IActionResult Test_ShipMethodSync()
        {
            try
            {
                var response = _associateWebService.GetShipMethods();
                var isUpdated = response.All(x => x.isUpdated);
                if (!isUpdated)
                {
                    _zloderZiplingoService.UpdateShipMethods(response);
                }
              return Ok(response);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
