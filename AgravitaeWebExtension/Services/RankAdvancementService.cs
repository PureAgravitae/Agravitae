using AgravitaeWebExtension.Models;
using AgravitaeWebExtension.Models.GenericReports;
using AgravitaeWebExtension.Repositories;
using Newtonsoft.Json;
using RestSharp;
using System.Data;

namespace AgravitaeWebExtension.Services
{
    public interface IRankAdvancementService
    {
        Task<RankAdvancementResponse> GetRankAdvancementDetail(int associateId);
    }
    public class RankAdvancementService : IRankAdvancementService
    {
        private readonly ILogger<RankAdvancementService> _logger;
        private readonly IRankAdvancementRepository _rankAdvancementRepository;

        public RankAdvancementService(ILogger<RankAdvancementService> logger, IRankAdvancementRepository rankAdvancementRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rankAdvancementRepository = rankAdvancementRepository ?? throw new ArgumentNullException(nameof(rankAdvancementRepository));
        }

        public async Task<RankAdvancementResponse> GetRankAdvancementDetail(int associateId)
        {
            return await _rankAdvancementRepository.GetRankAdvancementDetail(associateId);
        }

        private DataTable? UseNewtonsoftJson(string sampleJson )
        {
            DataTable? dataTable = new();
            if (string.IsNullOrWhiteSpace(sampleJson))
            {
                return dataTable;
            }
            dataTable = JsonConvert.DeserializeObject<DataTable>(sampleJson);
            return dataTable;
        }
    }
}
