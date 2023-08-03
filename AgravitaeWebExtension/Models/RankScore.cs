using System.Text.Json.Serialization;
namespace AgravitaeWebExtension.Models
{
    public class RankScore
    {
        [property: JsonPropertyName("RankID")]
        public int RankID { get; set; }
        [property: JsonPropertyName("RankName")]
        public string RankName { get; set; }
        [property: JsonPropertyName("Score")]
        public double Score { get; set; }
    }

    public class RankAdvancementResponse
    {
        [property: JsonPropertyName("AssociateID")]
        public int AssociateID { get; set; }
        [property: JsonPropertyName("HighestRankID")]
        public int HighestRankID { get; set; }
        [property: JsonPropertyName("HighestRankDescription")]
        public string? HighestRankDescription { get; set; }
        [property: JsonPropertyName("HighestRankAchievedDate")]
        public DateTime HighestRankAchievedDate { get; set; }
        [property: JsonPropertyName("RankID")]
        public int RankID { get; set; }
        [property: JsonPropertyName("RankDescription")]
        public string? RankDescription { get; set; }
        [property: JsonPropertyName("LastRankID")]
        public int LastRankID { get; set; }
        [property: JsonPropertyName("LastRankDescription")]
        public string? LastRankDescription { get; set; }
        [property: JsonPropertyName("LastCommissionRunDate")]
        public DateTime LastCommissionRunDate { get; set; }
        [property: JsonPropertyName("CurrentRank")]
        public string? CurrentRank { get; set; }
        [property: JsonPropertyName("Scores")]
        public RankScore[]? Scores { get; set; }

    }
}
