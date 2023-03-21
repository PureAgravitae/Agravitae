using DirectScale.Disco.Extension.Services;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using WebExtension.Merchants.Models;
using System.Linq;


namespace WebExtension.Merchants.EwalletMerchant.Ewallet
{
    public interface IEwalletRepository
    {
        EwalletSettings GetEwalletSettings();
        void UpdateEwalletSettings(EwalletSettingsRequest settings);
        void ResetEwalletSettings();

    }
    public class EwalletRepository : IEwalletRepository
    {
        private readonly IDataService _dataService;
        public EwalletRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public EwalletSettings GetEwalletSettings()
        {
            try
            {
                using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
                {
                    var settingsQuery = "SELECT * FROM Client.Ewallet_Settings";
                    return dbConnection.QueryFirstOrDefault<EwalletSettings>(settingsQuery);
                }
            }
            catch { return new EwalletSettings(); }
        }

        public void UpdateEwalletSettings(EwalletSettingsRequest settings)
        {
            var parameters = new
            {
                settings.CompanyId,
                settings.PointAccountId,
                settings.Username,
                settings.Password,
                settings.BackUpMerchantId,
                settings.SplitPayment,
                settings.ApiUrl
            };
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var query = @"UPDATE Client.Ewallet_Settings SET CompanyId = @CompanyId, PointAccountId = @PointAccountId, Username = @Username, Password = @Password, BackUpMerchantId = @BackUpMerchantId, SplitPayment = @SplitPayment, ApiUrl=@ApiUrl";

                dbConnection.Execute(query, parameters);
            }

        }

        public void ResetEwalletSettings()
        {
            try
            {
                var settings = GetEwalletSettings();
                var parameters = new
                {
                    Username = "helloapiuser",
                    Password = "7tE9TSUfgEMVKWU6",
                    ApiUrl = "https://rpmsapi.wsicloud.net/",
                    CompanyId = "624ecf9a20437a1e34406953",
                    PointAccountId = "624ed0d120437a1e34406958",
                    BackupMerchantId = 9012
                };

                using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
                {
                    var query = @"MERGE INTO Client.Ewallet_Settings WITH (HOLDLOCK) AS TARGET 
                USING 
                    (SELECT @Username AS 'Username', @Password AS 'Password', @ApiUrl AS 'ApiUrl', @CompanyId AS 'CompanyId', @PointAccountId AS 'PointAccountId', @BackupMerchantId as 'BackupMerchantId'
                ) AS SOURCE 
                    ON SOURCE.BackupMerchantId = TARGET.BackupMerchantId
                WHEN MATCHED THEN 
                    UPDATE SET TARGET.Username = SOURCE.Username, TARGET.Password = SOURCE.Password, TARGET.ApiUrl = SOURCE.ApiUrl, TARGET.CompanyId = SOURCE.CompanyId, TARGET.PointAccountId = SOURCE.PointAccountId
                WHEN NOT MATCHED BY TARGET THEN 
                    INSERT (Username, [Password], ApiUrl, CompanyId,PointAccountId) 
					VALUES (SOURCE.Username, SOURCE.Password, SOURCE.ApiUrl, SOURCE.CompanyId, SOURCE.PointAccountId);";

                    dbConnection.Execute(query, parameters);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
