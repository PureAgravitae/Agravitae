using Microsoft.AspNetCore.Mvc;
using System.Text;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Services;
using AgravitaeWebExtension.Models.Client_Requests;
using AgravitaeWebExtension.Models.GenericReports;
using AgravitaeWebExtension.Models;
using DirectScale.Disco.Extension.Services;
namespace AgravitaeWebExtension.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {
        private readonly ITreeService _treeService;
        private readonly IRankAdvancementService _rankAdvancementService;       

        public CustomerController(IRankAdvancementService rankAdvancementService, ITreeService treeService)
        {
            _rankAdvancementService = rankAdvancementService ?? throw new ArgumentNullException(nameof(rankAdvancementService));
            _treeService = treeService ?? throw new ArgumentNullException(nameof(treeService));
        }


        [HttpPost]
        [Route("GetRankAdvancementDetail")]
        public async Task<IActionResult> GetRankAdvancementDetail(int associateId)
        {
            try
            {
                var rankAdvancementList = new List<RankAdvancementResponse>();
                var retVal = new RankAdvancementResponse();

                var downline = await _treeService.GetDownlineIds(new DirectScale.Disco.Extension.NodeId() { AssociateId = associateId },
                                                                 DirectScale.Disco.Extension.TreeType.Unilevel,
                                                                 1);
                foreach(var id in downline)
                {
                    retVal = await _rankAdvancementService.GetRankAdvancementDetail(id.NodeId.AssociateId);
                    if(retVal != null) 
                        rankAdvancementList.Add(retVal);
                }
                
                return new Responses().OkResult(rankAdvancementList);
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
