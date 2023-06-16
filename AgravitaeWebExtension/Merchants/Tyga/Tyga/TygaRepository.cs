using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using AgravitaeExtension.Merchants.Tyga.Interfaces;
using AgravitaeExtension.Merchants.Tyga.Models;

namespace AgravitaeExtension.Merchants.Tyga.Tyga
{
    public class TygaRepository : ITygaRepository
    {
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;

        public TygaRepository(IDataService dataService, ISettingsService settingsService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }


        public string CreateOrder(string payorId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string cardNumber, string transactionNumber, string authorizationCode, string discoNotes)

        {
            //if (app.AssociateId == 0)
            //    app.AssociateId = responseAssociateId;
            //try
            //{
            //    var settings = _TygaRepository.GetSettings();
            //    Ewallet.Models.CustomerDetails request = new Ewallet.Models.CustomerDetails
            //    {
            //        FirstName = app.FirstName,
            //        LastName = app.LastName,
            //        ExternalCustomerID = app.AssociateId.ToString(),
            //        BackofficeID = app.BackOfficeId,
            //        CompanyID = settings.CompanyId,
            //        EmailAddress = app.EmailAddress,
            //        PhoneNumber = app.PrimaryPhone,
            //        DateOfBirth = app.BirthDate,
            //        CustomerLanguage = app.LanguageCode,
            //        CountryCode = app.ApplicantAddress.CountryCode,
            //    };

            //    CallEwallet("api/Customer/CreateCustomer", request);
            //}
            //catch { }
            return "";
        }

        public async Task SaveErrorLogResponse(int associateId, int orderId, string message, string error)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    associateId,
                    orderId,
                    message,
                    error
                };
                var insertStatement = @"INSERT INTO Client.TygaLogResponse(AssociateID,OrderID,Message,Error) VALUES(@associateId,@orderId,@message,@error)";
                dbConnection.Execute(insertStatement, parameters);
            }
        }
        public async Task CreateTygaOrderLogs(int associateId, int orderId, CreateTygaOrderResponse req)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    associateId,
                    orderId,
                    message = req.Message,
                    TygaOrderId = req.Data.OrderId,
                    PaymentUrl = req.Data.PaymentUrl
                };
                var insertStatement = @"INSERT INTO Client.TygaOrders(AssociateID,OrderID,Message,TygaOrderId,PaymentUrl) VALUES(@associateId,@orderId,@message,@TygaOrderId,@PaymentUrl)";
                dbConnection.Execute(insertStatement, parameters);
            }
        }
        public async Task UpdateTygaOrderLogs(TygaPaymentResponse req)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    status = req.Status,
                    TygaOrderId = req.OrderId,
                    TransactionNumber = req.TxId,
                    CompleteResponse = Newtonsoft.Json.JsonConvert.SerializeObject(req)

                };
                var updateStatement = @"update Client.TygaOrders  set status = @status, TransactionNumber= @TransactionNumber, CompleteResponse = @CompleteResponse where TygaOrderId = @TygaOrderId";
                dbConnection.Execute(updateStatement, parameters);
            }
        }
        public async Task<TygaOrder> GetTygaOrderbyOrderId(string orderId)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    TygaOrderId = orderId
                };
                var selectStatement = @"Select * from Client.TygaOrders where TygaOrderId = @TygaOrderId ";
                return dbConnection.QueryFirstOrDefault<TygaOrder>(selectStatement, parameters);

            }
        }

        public async Task<TygaSettings> GetTygaSettings()
        {
            try
            {
                EnvironmentType env = _settingsService.ExtensionContext().GetAwaiter().GetResult().EnvironmentType;
                using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
                {
                    var parameters = new
                    {
                        Environment = (env == EnvironmentType.Live ? "Live" : "Stage")
                    };
                    var settingsQuery = "SELECT * FROM Client.Tyga_Settings where Environment = @Environment";
                    return dbConnection.QueryFirstOrDefault<TygaSettings>(settingsQuery, parameters);
                }
            }
            catch { return new TygaSettings(); }
        }

    }
}
