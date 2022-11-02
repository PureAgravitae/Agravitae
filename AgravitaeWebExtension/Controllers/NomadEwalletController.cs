using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebExtension.Helper;
using WebExtension.Model;
using WebExtension.Models;
using WebExtension.Services;

namespace WebExtension.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NomadEwalletController : ControllerBase
    {
        private readonly INomadEwalletService _nomadEwalletService;
        public NomadEwalletController(
            INomadEwalletService nomadEwalletService
        )
        {
            _nomadEwalletService = nomadEwalletService ?? throw new ArgumentNullException(nameof(nomadEwalletService));
        }


        [HttpPost]
        [Route("GetNomadEwalletAccountBalance")]
        public async Task<IActionResult> GetNomadEwalletAccountBalance([FromBody] GetNomadEwalletAccountBalanceRequest request)
        {
            try
            {
                return new Responses().OkResult(await _nomadEwalletService.GetNomadEwalletAccountBalance(request));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetSingleSignON")]
        public async Task<IActionResult> GetSingleSignON([FromBody] SingleSignOnRequest request)
        {
            try
            {
                return new Responses().OkResult(await _nomadEwalletService.SingleSignOn(request));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
