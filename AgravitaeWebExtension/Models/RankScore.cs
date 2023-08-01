namespace AgravitaeWebExtension.Models
{
    public class RankScore
    {
        public int RankID { get; set; }
        public string RankName { get; set; }
        public double Score { get; set; }
    }

    public class RankAdvancementResponse
    {
        public int AssociateID { get; set; }
        public int HighestRankID { get; set; }
        public string HighestRankDescription { get; set; }
        public DateTime HighestRankAchievedDate { get; set; }
        public int RankID { get; set; }
        public string RankDescription { get; set; }
        public int LastRankID { get; set; }
        public string LastRankDescription { get; set; }
        public DateTime LastCommissionRunDate { get; set; }
        public string CurrentRank { get; set; }
        public RankScore[] Scores { get; set; }

    }
}
