using DirectScale.Disco.Extension.Services;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using AgravitaeWebExtension.Merchants.Models;
using System.Linq;

namespace AgravitaeWebExtension.Merchants
{
    public interface IClientRepository
    {
        List<CommissionMerchantAssociateValues> GetAchInfo(List<int> associateIds);
        List<CommissionMerchantAssociateValues> GetIPayoutInfo(List<int> associateIds);
        List<CommissionMerchantAssociateValues> GetAssociateMerchantInfo(int associateId, int merchantId);
        int GetAssociateActiveCommissionMerchant(int associateId);
    }
    public class ClientRepository : IClientRepository
    {
        private readonly IDataService _dataService;

        public ClientRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public List<CommissionMerchantAssociateValues> GetAchInfo(List<int> associateIds)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new { associateIds = associateIds };
                var query = "SELECT * FROM CRM_CommissionMerchant_AssociateValues WHERE MerchantID = 114 AND AssociateID in @associateIds";

                return dbConnection.Query<CommissionMerchantAssociateValues>(query, parameters).ToList();
            }
        }

        public List<CommissionMerchantAssociateValues> GetIPayoutInfo(List<int> associateIds)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new { associateIds = associateIds };
                var query = "SELECT * FROM CRM_CommissionMerchant_AssociateValues WHERE MerchantID = 112 AND AssociateID in @associateIds";

                return dbConnection.Query<CommissionMerchantAssociateValues>(query, parameters).ToList();
            }
        }

        public List<CommissionMerchantAssociateValues> GetAssociateMerchantInfo(int associateId, int merchantId)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new { associateId = associateId, merchantId = merchantId };
                var query = "SELECT * FROM CRM_CommissionMerchant_AssociateValues WHERE MerchantID = @merchantId AND AssociateID = @associateId";

                return dbConnection.Query<CommissionMerchantAssociateValues>(query, parameters).ToList();
            }
        }

        public int GetAssociateActiveCommissionMerchant(int associateId)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var query = "SELECT MerchantID FROM CRM_CommissionMerchant_Active WHERE AssociateID = @associateId";

                return dbConnection.QueryFirstOrDefault<int>(query, new { associateId });
            }
        }

    }
}
