using System;
using AgravitaeWebExtension.Services.ZiplingoEngagementService.Model;
using System.Collections.Generic;
using Dapper;
using DirectScale.Disco.Extension.Services;
using System.Linq;
using AgravitaeWebExtension.Services.ZiplingoEngagement.Model;
using WebExtension.Models;
using AgravitaeWebExtension.Services.ZiplingoEngagementService.Model;

namespace AgravitaeWebExtension.Services.ZiplingoEngagement
{
    public interface IZiplingoEngagementRepository
    {
        ZiplingoEngagementSettings GetSettings();
        EWalletSettingModel GetEWalletSetting();
        List<ZiplingoEventSettings> GetEventSettingsList();
        void UpdateSettings(ZiplingoEngagementSettingsRequest settings);
        void UpdateEventSetting(ZiplingoEventSettingRequest request);

    }
    public class ZiplingoEngagementRepository : IZiplingoEngagementRepository
    {
        private readonly IDataService _dataService;

        public ZiplingoEngagementRepository(
            IDataService dataService
            )
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }



        public ZiplingoEngagementSettings GetSettings()
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var settingsQuery = "SELECT * FROM Client.ZiplingoEngagementSettings";

                return dbConnection.QueryFirstOrDefault<ZiplingoEngagementSettings>(settingsQuery);
            }
        }
        public EWalletSettingModel GetEWalletSetting()
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var settingsQuery = "SELECT * FROM Client.Ewallet_Settings";

                return dbConnection.QueryFirstOrDefault<EWalletSettingModel>(settingsQuery);
            }
        }

        public List<ZiplingoEventSettings> GetEventSettingsList()
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameter = new { };
                var query = @"SELECT * FROM Client.ZiplingoEventSettings ORDER BY recordnumber DESC";
                var result = dbConnection.Query<ZiplingoEventSettings>(query, parameter).ToList();
                return result;
            }
        }
        public void UpdateEventSetting(ZiplingoEventSettingRequest request)
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    eventKey = request.eventKey,
                    Status = request.Status
                };

                var updateStatement = @"UPDATE Client.ZiplingoEventSettings SET Status = @Status WHERE eventKey = @eventKey";
                dbConnection.Execute(updateStatement, parameters);
            }
        }


        public void UpdateSettings(ZiplingoEngagementSettingsRequest settings)
        {
            using (var dbConnection = new System.Data.SqlClient.SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    settings.ApiUrl,
                    settings.Username,
                    settings.Password,
                    settings.LogoUrl,
                    settings.CompanyName
                };

                var updateStatement = @"UPDATE Client.ZiplingoEngagementSettings SET ApiUrl = @ApiUrl,  Username = @Username, Password = @Password, LogoUrl = @LogoUrl, CompanyName = @CompanyName";
                dbConnection.Execute(updateStatement, parameters);
            }
        }
    }
}
