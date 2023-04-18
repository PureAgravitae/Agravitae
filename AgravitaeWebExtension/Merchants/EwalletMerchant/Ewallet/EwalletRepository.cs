using DirectScale.Disco.Extension.Services;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using WebExtension.Merchants.Models;
using System.Linq;
using DirectScale.Disco.Extension;

namespace WebExtension.Merchants.EwalletMerchant.Ewallet
{
    public interface IEwalletRepository
    {
        EwalletSettings GetEwalletSettings();
        void UpdateEwalletSettings(EwalletSettingsRequest settings);
        void ResetEwalletSettings();
        void SaveErrorLogResponse(int associateId, int orderId, string message, string error);

    }
    public class EwalletRepository : IEwalletRepository
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        public EwalletRepository(IDataService dataService, ISettingsService settingsService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }
        public EwalletSettings GetEwalletSettings()
        {
            try
            {
                EnvironmentType env = _settingsService.ExtensionContext().Result.EnvironmentType;
                using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
                {
                    var parameters = new
                    {
                        Environment = (env == EnvironmentType.Live ? "Live" : "Stage")
                    };
                    var settingsQuery = "SELECT * FROM Client.Ewallet_Settings where Environment = @Environment";
                    return dbConnection.QueryFirstOrDefault<EwalletSettings>(settingsQuery, parameters);
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
                    Username = "MPGXtremeApiUser",
                    Password = "D3kX42mIuZeh",
                    ApiUrl = "https://rpmsapi.wsicloud.net/",
                    CompanyId = "642544e41a2f620aac513863",
                    PointAccountId = "642545541a2f620aac513866",
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
        public void SaveErrorLogResponse(int associateId, int orderId, string message, string error)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    associateId,
                    orderId,
                    message,
                    error
                };
                var insertStatement = @"INSERT INTO Client.CheckErrorLogResponse(AssociateID,OrderID,Message,Error) VALUES(@associateId,@orderId,@message,@error)";
                dbConnection.Execute(insertStatement, parameters);
            }
        }
    }
}
