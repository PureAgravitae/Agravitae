using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using RPMSEwallet.Services;
using RPMSEwallet.Services.Interface;

namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IEwalletService _ewalletService;

        public TestApiController(IDataService dataService, IEwalletService ewalletService)
        {
            _dataService = dataService;
            _ewalletService = ewalletService;
        }

        [HttpGet]
        [Route("GetDatabaseConnection")]
        public IActionResult TestApi()
        {
            var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result);
            return Ok(dbConnection.ConnectionString);
        }
        [HttpGet]
        [Route("UpdateEwalletSetting")]
        public IActionResult UpdateEwalletSetting(RPMSEwallet.Models.EwalletSettingsRequest request)
        {
             _ewalletService.UpdateEwalletSettings(request);
            return Ok();
        }


    }
}
