using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Models;

namespace AgravitaeAgravitaeWebExtension.Controllers
{
    public class ConnectionStringController  : ControllerBase
    {
        private readonly IDataService _dataService;
        public ConnectionStringController(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        [HttpGet]
        [Route("GetConnectionString")]
        public async Task<IActionResult> GetConnectionString()
        {
            try
            {
                return new Responses().OkResult(await _dataService.GetClientConnectionString());
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
