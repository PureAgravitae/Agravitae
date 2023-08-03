using Microsoft.AspNetCore.Mvc;
using System.Text;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Services;
using AgravitaeWebExtension.Models.Client_Requests;
using AgravitaeWebExtension.Models.GenericReports;
using AgravitaeWebExtension.Models;
using DirectScale.Disco.Extension.Services;
using System.Data;

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
        [Route("GetRankAdvancementDetails")]
        public async Task<IActionResult> GetRankAdvancementDetails(int associateId)
        {
            try
            {
                var rankAdvancementList = new List<RankAdvancementResponse>();
                var retVal = new RankAdvancementResponse();

                var downline = await _treeService.GetDownlineIds(new DirectScale.Disco.Extension.NodeId() { AssociateId = associateId },
                                                                 DirectScale.Disco.Extension.TreeType.Unilevel,
                                                                 1);
                //Creating dummy datatable for testing
                DataTable dt = new DataTable("RankAdvancement");
                DataColumn dc = new DataColumn("Name", typeof(String));
                dt.Columns.Add(dc);

                dc = new DataColumn("CurrentRank", typeof(String));
                dt.Columns.Add(dc);

                dc = new DataColumn("NextRank", typeof(String));
                dt.Columns.Add(dc);

                dc = new DataColumn("PercentAdvanced", typeof(String));
                dt.Columns.Add(dc);
                int counter = 0;


                foreach (var id in downline)
                {
                    var assoc = await _associateService.GetAssociate(id.NodeId.AssociateId);
                    if (assoc != null)
                    {
                        if(assoc.StatusId.Equals(1) && assoc.AssociateType.Equals(1))
                        {
                            retVal = await _rankAdvancementService.GetRankAdvancementDetail(id.NodeId.AssociateId);
                            if (retVal != null && retVal.AssociateID > 0)
                            {
                                DataRow dr = dt.NewRow();

                                dr[0] = assoc.Name;


                                foreach (var rankItem in retVal.Scores)
                                {

                                    if (counter == 0)
                                    {
                                        dr[1] = rankItem.RankName;
                                        counter++;
                                        continue;
                      
                                    }

                                    if (counter == 1)
                                    {

                                        dr[2] = rankItem.RankName;
                                        dr[3] = rankItem.Score;
                                        counter = 0;

                                        dt.Rows.Add(dr);
                                        break;
                                    }

                                }
                                rankAdvancementList.Add(retVal);
                            }

                        }
                    }

                }

                string result = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                return new Responses().OkResult(result);
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
