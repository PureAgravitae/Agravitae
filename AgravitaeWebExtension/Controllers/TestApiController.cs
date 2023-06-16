using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestApiController : ControllerBase
    {
        private readonly IDataService _dataService;

        public TestApiController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [Route("GetDatabaseConnection")]
        public IActionResult TestApi()
        {
            var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result);
            return Ok(dbConnection);
        }
    }
}
