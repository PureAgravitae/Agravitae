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
                var retVal = new RankAdvancementResponse();
                var downline = await _treeService.GetDownlineIds(new DirectScale.Disco.Extension.NodeId() { AssociateId = associateId },
                                                                 DirectScale.Disco.Extension.TreeType.Unilevel,
                                                                 1);
                var result = new QueryResult()
                {
                    Columns = new List<ColumnInfo>(),
                    Rows = new List<List<string>>()
                };
                List<string> row = new List<string>();

                result.Columns.Add(new ColumnInfo()
                {
                    ColumnName = "Name",
                    DataType = SqlDataType.String
                });

                result.Columns.Add(new ColumnInfo()
                {
                    ColumnName = "CurrentRank",
                    DataType = SqlDataType.String
                });

                result.Columns.Add(new ColumnInfo()
                {
                    ColumnName = "NextRank",
                    DataType = SqlDataType.String
                });

                result.Columns.Add(new ColumnInfo()
                {
                    ColumnName = "PercentAdvanced",
                    DataType = SqlDataType.String
                });
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
                                List<string> alertRow = new List<string>();
                                alertRow.Add(assoc.Name);

                                foreach (var rankItem in retVal.Scores)
                                {
                                    if (counter == 0)
                                    {
                                        alertRow.Add(rankItem.RankName);
                                        counter++;
                                        continue;
                      
                                    }
                                    if (counter == 1)
                                    {
                                        int scoreTruncated = (int)rankItem.Score;

                                        alertRow.Add(rankItem.RankName);
                                        alertRow.Add(scoreTruncated.ToString());
                                        counter = 0;
                                        result.Rows.Add(alertRow);
                                        break;
                                    }

                                }
                            }
                        }
                    }

                }
                return new Responses().OkResult(result);
                
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
