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
        private readonly IAssociateService _associateService;

        public CustomerController(IRankAdvancementService rankAdvancementService, ITreeService treeService, IAssociateService associateService)
        {
            _rankAdvancementService = rankAdvancementService ?? throw new ArgumentNullException(nameof(rankAdvancementService));
            _treeService = treeService ?? throw new ArgumentNullException(nameof(treeService));
            _associateService = associateService;
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
                foreach (var id in downline)
                {
                    var assoc = await _associateService.GetAssociate(id.NodeId.AssociateId);
                    if (assoc != null)
                    {
                        if(assoc.StatusId.Equals(1) && assoc.AssociateType.Equals(1))
                        {
                            retVal = await _rankAdvancementService.GetRankAdvancementDetail(id.NodeId.AssociateId);
                            if (retVal != null && retVal.AssociateID > 0)
                                rankAdvancementList.Add(retVal);
                        }
                    }

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
