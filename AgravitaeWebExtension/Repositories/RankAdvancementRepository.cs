using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Models;
using AgravitaeWebExtension.Models.GenericReports;
using DirectScale.Disco.Extension.Services;
using System.Data.SqlClient;
using WebExtension.Reports;

namespace AgravitaeWebExtension.Repositories
{
    public interface IRankAdvancementRepository
    {
        Task<RankAdvancementResponse> GetRankAdvancementDetail(int associateId);
    }
    public class RankAdvancementRepository : IRankAdvancementRepository
    {
        private readonly IStatsService _statsService;
        private readonly IRankService _rankService;
        private readonly IHistoryService _historyService;

        public RankAdvancementRepository(IStatsService statsService, IRankService rankService, IHistoryService historyService)
        {
            _statsService = statsService ?? throw new ArgumentNullException(nameof(statsService));
            _rankService = rankService;
            _historyService = historyService;
        }

        public async Task<RankAdvancementResponse> GetRankAdvancementDetail(int associateId)
        {
            try
            {
                var retVal = new RankAdvancementResponse();
                var scores = new List<RankScore>();
                var highRank = await _historyService.GetHighRankDate(associateId);
                var lastRank = await _historyService.GetLastRankDate(associateId);                

                //if (highRank.Rank == 0)
                //    return retVal;
                var statInfo = await _statsService.GetStats(new int[] { associateId }, DateTime.Now);
                var stats = statInfo.Values.Select(x => x.Ranks).FirstOrDefault();

                if (stats != null && stats.Length > 0)
                {
                    int highestCurrentRankId = 0;
                    int count75 = 0, noRankCount = 0;
                    foreach (var option in stats)
                    {
                        if (count75 == 1 || noRankCount == 1)
                            break;

                        var rankScore = new RankScore
                        {
                            RankID = option.RankId,
                            RankName = await _rankService.GetRankName(option.RankId)
                        };

                        //All they need to do to qualify is fulfill all of the "Options" in a given group
                        foreach (var rankGroup in option.Groups)
                        {
                            if (rankGroup != null && rankGroup.Details.Length > 0)
                            {
                                //int pass = 0;
                                double pctScore = 0;
                                foreach (var rankOption in rankGroup.Details)
                                {
                                    pctScore += rankOption.PercentComplete;
                                }

                                double currScore = (pctScore / rankGroup.Details.Length) * 100;

                                if (currScore < 75)
                                {
                                    if(currScore == 0)
                                    {
                                        noRankCount++;
                                    }
                                    continue;
                                }

                                if (currScore > 100)
                                {
                                    currScore = 100;
                                }

                                if (currScore >= 75)
                                {
                                    if (currScore != 100)
                                    {
                                        count75++;
                                    }
                                }

                                if (currScore > rankScore.Score)
                                {
                                    rankScore.Score = currScore;
                                }

                                //If all pass, we're done. Break the loop
                                if (currScore == 100)
                                {
                                    if (option.RankId > highestCurrentRankId)
                                    {
                                        highestCurrentRankId = option.RankId;
                                    }

                                    break;
                                }
                            }
                        }

                        if (rankScore.Score >= 75 && highestCurrentRankId > 0)
                        {
                            var hundredScore = scores.Where(h => h.Score.Equals(100)).FirstOrDefault();
                            if(hundredScore != null && rankScore.Score == 100)
                            {
                                scores.Remove(hundredScore);
                            }

                            scores.Add(rankScore);

                        }
                    }

                    if (scores.Count > 1)
                    {
                        retVal.AssociateID = associateId;
                        retVal.HighestRankID = highestCurrentRankId;
                        retVal.HighestRankDescription = await _rankService.GetRankName(highestCurrentRankId);
                        retVal.HighestRankAchievedDate = highRank.Date;
                        retVal.RankID = highestCurrentRankId;
                        retVal.RankDescription = await _rankService.GetRankName(highestCurrentRankId);
                        retVal.LastRankID = lastRank.Rank;
                        retVal.LastRankDescription = await _rankService.GetRankName(lastRank.Rank);
                        retVal.LastCommissionRunDate = lastRank.Date;
                        retVal.CurrentRank = await _rankService.GetRankName(highestCurrentRankId);
                        retVal.Scores = scores.ToArray();
                    }
                }


                return retVal;
            }
            catch (Exception)
            {
                return new RankAdvancementResponse();
            }
        }
    }
}
