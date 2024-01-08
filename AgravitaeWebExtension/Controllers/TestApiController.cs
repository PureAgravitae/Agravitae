using AgravitaeWebExtension.Services;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using ZiplingoEngagement.Services.Interface;

namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private readonly IDataService _dataService;
        private readonly IAssociateWebService _associateWebService;
        private readonly IZLOrderZiplingoService _zloderZiplingoService;

        public TestApiController(IDataService dataService, IAssociateWebService associateWebService, IZLOrderZiplingoService zloderZiplingoService)
        {
            _dataService = dataService;
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
