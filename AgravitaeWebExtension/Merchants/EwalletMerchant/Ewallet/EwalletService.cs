using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AgravitaeWebExtension.Merchants.EwalletMerchant.Models;
using System;
using System.Net.Http;
using System.Text;
using AgravitaeWebExtension.Helper;
using AgravitaeWebExtension.Merchants.Models;

namespace AgravitaeWebExtension.Merchants.EwalletMerchant.Ewallet
{
    public interface IEwalletService
    {
        void UpdateCustomer(DirectScale.Disco.Extension.Associate req);
        string CreateCustomer(Application app, int responseAssociateId);
        dynamic CallEwallet(string requestUrl, object requestData);
        PointBalanceResponse GetPointBalance(GetPointBalanceRequest request);
        CreatePointAccountTransaction CreatePointTransaction(CustomerPointTransactionsRequest request);
        string CreateToken();
        PaymentResponse CreditPayment(string payorId, int orderNumber, string currencyCode, decimal paymentAmount, decimal refundAmount, string cardNumber, string transactionNumber, string authorizationCode);
        int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest request);
        EwalletSettings GetEwalletSettings();
        void UpdateEwalletSettings(EwalletSettingsRequest settings);
        void ResetEwalletSettings();
        void SaveErrorLogResponse(int associateId, int orderId, string message, string error);
    }

    public class EwalletService : IEwalletService
    {
        //private readonly ILogger _logger;
        private readonly IClientService _clientService;
        private static readonly string _className = typeof(EwalletService).FullName;
        private readonly IHttpClientService _httpClientService;
        private readonly IMoneyOutService _moneyOutService;
        private readonly ICurrencyService _currencyService;
        private readonly IAssociateService _associateService;
        private readonly IEwalletRepository _ewalletRepository;

        public EwalletService(IClientService clientService, IHttpClientService httpClientService, IMoneyOutService moneyOutService, ICurrencyService currencyService, IAssociateService associateService, IEwalletRepository ewalletRepository)
        {
            _clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            _moneyOutService = moneyOutService ?? throw new ArgumentNullException(nameof(moneyOutService));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _ewalletRepository = ewalletRepository ?? throw new ArgumentNullException(nameof(ewalletRepository));
        }

        public string CreateCustomer(Application app, int responseAssociateId)
        {
            if (app.AssociateId == 0)
                app.AssociateId = responseAssociateId;
            try
            {
                var settings = GetEwalletSettings();
                CustomerDetails request = new CustomerDetails
                {
                    FirstName = app.FirstName,
                    LastName = app.LastName,
                    ExternalCustomerID = app.AssociateId.ToString(),
                    BackofficeID = app.BackOfficeId,
                    CompanyID = settings.CompanyId,
                };

                CallEwallet("api/Customer/CreateCustomer", request);
            }
            catch { }
            return app.AssociateId.ToString();
        }

        public void UpdateCustomer(DirectScale.Disco.Extension.Associate req)
        {
            try
            {
                var settings = GetEwalletSettings();
                CustomerDetails request = new CustomerDetails
                {
                    FirstName = req.DisplayFirstName,
                    LastName = req.DisplayLastName,
                    ExternalCustomerID = req.AssociateId.ToString(),
                    BackofficeID = req.BackOfficeId,
                    CompanyID = settings.CompanyId,
                };
                CallEwallet("api/Customer/UpdateCustomer", request);
            }
            catch { }
        }

        public string CreateToken()
        {
            try
            {
                var settings = GetEwalletSettings();
                TokenRequest trequest = new TokenRequest { client_id = settings.CompanyId, username = settings.Username, password = settings.Password };
                var jsonData = JsonConvert.SerializeObject(trequest);

                var apiUrl = settings.ApiUrl + "token";

                var data = _httpClientService.PostRequest(apiUrl, trequest);

                if (data?.StatusCode.ToString() == "OK")
                {
                    var jsonString = data?.Content?.ReadAsStringAsync();
                    jsonString.Wait();

                    var jobject = jsonString?.Result?.ToString();
                    dynamic jdata = JObject.Parse(jobject);
                    return jdata?.access_token;
                }
                else
                {
                    throw new Exception(data?.StatusCode.ToString() + data?.Content.ToString() + " URL: " + apiUrl + " json: " + jsonData + " reason: " + data.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public dynamic CallEwallet(string requestUrl, object requestData)
        {
            try
            {
                string token = CreateToken();
                var settings = GetEwalletSettings();

                if (!string.IsNullOrEmpty(token))
                {
                    token = "Bearer " + token;
                }
                else
                {
                    throw new Exception($"Error occured at Method {System.Reflection.MethodBase.GetCurrentMethod().Name} and Error = Token is NULL!");
                }

                var jsonData = JsonConvert.SerializeObject(requestData);
                var apiUrl = settings.ApiUrl + requestUrl;
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(apiUrl));
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var result = _httpClientService.MakeRequestByToken(request, "Authorization", token);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return result;
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new Exception($"Error occured at Method {System.Reflection.MethodBase.GetCurrentMethod().Name} and Error = Invalid Token!");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occured at Method {System.Reflection.MethodBase.GetCurrentMethod().Name} and error ='{ex.Message}'");
            }
            return null;
        }

        public PointBalanceResponse GetPointBalance(GetPointBalanceRequest request)
        {
            try
            {
                var settings = GetEwalletSettings();
                GetPointBalanceRequest brequest = new GetPointBalanceRequest
                {
                    ExternalCustomerId = request.ExternalCustomerId,
                    CompanyId = settings.CompanyId,
                    PointAccountId = settings.PointAccountId
                };
                var response = CallEwallet("api/CustomerPointBalances/GetCustomerPointBalances", brequest);
                var jsonString = response?.Content?.ReadAsStringAsync();

                jsonString.Wait();
                var jobject = jsonString?.Result;
                dynamic data = JObject.Parse(jobject);

                if (data.status == "Success" && data.data != null)
                {
                    return new PointBalanceResponse
                    { Amount = data.data.amount, HoldAmount = data.data.holdAmount };
                }
            }
            catch (Exception e)
            {
                //_logger.Log(LogLevel.Error, "{1} {2}", e.Message, e.StackTrace);
            }
            return new PointBalanceResponse();
        }

        public CreatePointAccountTransaction CreatePointTransaction(CustomerPointTransactionsRequest request)
        {
            try
            {
                var settings = GetEwalletSettings();

                request.CompanyId = settings.CompanyId;
                request.PointAccountID = settings.PointAccountId;

                var response = CallEwallet("api/CustomerPointTransactions/CreatePointTransaction", request);
                var jsonString = response?.Content?.ReadAsStringAsync();
                jsonString.Wait();
                var jobject = jsonString?.Result;
                dynamic data = JObject.Parse(jobject);
                if (data.status == "Success" && data.data != null)
                {
                    return new CreatePointAccountTransaction { TransactionNumber = data.data, Status = data.status }; ;
                }
                else
                {
                    SaveErrorLogResponse(0, 0, "CreatePointTransaction Error", "Error in CreatePointTransaction " + "Response: " + jsonString);
                    return new CreatePointAccountTransaction { TransactionNumber = "", Status = "failed" };
                }
            }
            catch (Exception ex)
            {
                SaveErrorLogResponse(0, 0, "CreatePointTransaction Exception", "Exception in CreatePointTransaction " + "Error: " + ex.InnerException);
                return new CreatePointAccountTransaction { TransactionNumber = "", Status = "failed" };

            }

        }

        public PaymentResponse CreditPayment(string payorId, int orderNumber, string currencyCode, decimal paymentAmount, decimal refundAmount, string cardNumber, string transactionNumber, string authorizationCode)
        {
            var res = new PaymentResponse();
            CreatePointAccountTransaction response = new CreatePointAccountTransaction();
            paymentAmount = _currencyService.Round(paymentAmount, currencyCode).Result;
            refundAmount = _currencyService.Round(refundAmount, currencyCode).Result;

            var refundTrans = new RefundTransaction
            {
                Id = transactionNumber,
                RefundData = new RefundData
                {
                    Amount = paymentAmount,
                    PartialAmount = refundAmount,
                    Currency = currencyCode.ToUpper()
                }
            };

            try
            {
                var paymentSource = "";
                RedeemType redeemType = RedeemType.Commission;
                if (authorizationCode == "commission")
                {
                    paymentSource = $"Commissions from Associate:{payorId} Amount:{paymentAmount.ToString()}";
                    redeemType = RedeemType.Commission;
                }
                if (authorizationCode == "refund")
                {
                    paymentSource = $"Refund Amount:{refundAmount.ToString()} to Associate:{payorId} on Order:{orderNumber}";
                    redeemType = RedeemType.Refund;
                }
                CustomerPointTransactionsRequest data = new CustomerPointTransactionsRequest
                {
                    Amount = refundTrans.RefundData.Amount,
                    ExternalCustomerID = payorId,
                    RedeemType = redeemType,
                    TransactionType = TransactionType.Credit,
                    ReferenceNo = transactionNumber,
                    Comment = paymentSource
                };

                if (!refundTrans.RefundData.Amount.Equals(refundTrans.RefundData.PartialAmount))
                {
                    data.Amount = refundTrans.RefundData.PartialAmount;
                }

                response = CreatePointTransaction(data);

                if (!string.IsNullOrEmpty(response.Status))
                {
                    if (response.Status.Contains("error") || response.Status.Contains("failed"))
                    {
                        res.TransactionNumber = "";
                    }
                    else
                    {
                        res.Status = PaymentStatus.Accepted;
                        res.TransactionNumber = response.TransactionNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                SaveErrorLogResponse(0, 0, "CreditPayment TO E-Wallet Exception", "Exception in CreditPaymen" + "Error: " + ex.InnerException);
            }

            SaveErrorLogResponse(int.Parse(payorId), orderNumber, "Credit Payment", $"Processed refund for order {orderNumber}. TransactionId: {transactionNumber}, Amount: {refundTrans.RefundData.Amount}, Returned status: {response}.");

            return new PaymentResponse
            {
                Amount = (double)refundAmount,
                AuthorizationCode = authorizationCode,
                Currency = currencyCode.ToUpper(),
                OrderNumber = orderNumber,
                TransactionNumber = transactionNumber,
                ResponseId = "0",
                PaymentType = "Credit",
                Response = response.Status,
                Status = response.Status.Equals("success", StringComparison.OrdinalIgnoreCase) ? PaymentStatus.Accepted : PaymentStatus.Rejected
            };
        }

        public int SetActiveCommissionMerchant(SetActiveCommissionMerchantRequest request)
        {
            try
            {
                var result = _moneyOutService.AutoProvisionAccount(request.AssociateId, request.MerchantId).Result;

                if (result?.MerchantId != 0)
                {
                    return 1;
                }
            }
            catch { }

            return 0;
        }

        public EwalletSettings GetEwalletSettings()
        {
            return _ewalletRepository.GetEwalletSettings();
        }

        public void UpdateEwalletSettings(EwalletSettingsRequest settings)
        {
            _ewalletRepository.UpdateEwalletSettings(settings);
        }

        public void ResetEwalletSettings()
        {
            _ewalletRepository.ResetEwalletSettings();
        }
        public void SaveErrorLogResponse(int associateId, int orderId, string message, string error)
        {
            _ewalletRepository.SaveErrorLogResponse(associateId, orderId, message, error);
        }
    }
}
