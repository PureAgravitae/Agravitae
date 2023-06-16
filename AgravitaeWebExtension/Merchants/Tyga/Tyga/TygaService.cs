using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AgravitaeExtension.Merchants.Tyga.Models;
using System;
using System.Net.Http;
using System.Text;
using AgravitaeExtension.Merchants.Tyga.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AgravitaeWebExtension.Helper;

namespace AgravitaeExtension.Merchants.Tyga.Tyga
{
    public class TygaService : ITygaService
    {
        private readonly ITygaRepository _TygaRepository;
        private static readonly string _className = typeof(TygaService).FullName;
        private readonly IHttpClientService _httpClientService;
        private readonly IMoneyOutService _moneyOutService;
        private readonly ICurrencyService _currencyService;
        private readonly IAssociateService _associateService;
        private readonly IOrderService _orderService;

        public TygaService(ITygaRepository repository, IHttpClientService httpClientService, IMoneyOutService moneyOutService, ICurrencyService currencyService, IAssociateService associateService, IOrderService orderService)
        {
            _TygaRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _httpClientService = httpClientService ?? throw new ArgumentNullException(nameof(httpClientService));
            _moneyOutService = moneyOutService ?? throw new ArgumentNullException(nameof(moneyOutService));
            _currencyService = currencyService ?? throw new ArgumentNullException(nameof(currencyService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }


        public async Task<CreateTygaOrderResponse> CreateOrder(string requestUrl, CreateTygaOrderRequest requestData)
        {
            CreateTygaOrderResponse response = new CreateTygaOrderResponse();
            try
            {
                var settings = await _TygaRepository.GetTygaSettings();
                if (requestData.returnUrl.Contains("OrderCheckout"))
                {
                    requestData.returnUrl = settings.ReturnUrl;
                }
                requestData.notifyUrl = settings.NotifyUrl;
                requestData.returnUrl = requestData.returnUrl;
                var jsonData = JsonConvert.SerializeObject(requestData,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var apiUrl = settings.ApiBaseUrl + requestUrl;
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), new Uri(apiUrl));
                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var bodyHash = GenerateHash(requestData, settings.ApiSecret);
                request.Headers.Add("x-api-key", settings.ApiKey);
                request.Headers.Add("x-api-hash", bodyHash);
                var result = _httpClientService.MakePostRequest(request);
                var jsonString = result?.Content?.ReadAsStringAsync();

                jsonString.Wait();
                var jobject = jsonString?.Result;
                dynamic data = JObject.Parse(jobject);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await SaveErrorLogResponse(00, 0, "createTygaOrerRequest Success", "result:- " + Newtonsoft.Json.JsonConvert.SerializeObject(jsonString.Result));

                    response = new CreateTygaOrderResponse
                    {
                        Message = "Success",
                        Data = new CreateOrderResponseData
                        {
                            OrderId = data.data.orderId,
                            PaymentUrl = data.data.paymentUrl
                        }
                    };
                }
                else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await SaveErrorLogResponse(00, 0, "createTygaOrerRequest Unauthorized", "result:- " + Newtonsoft.Json.JsonConvert.SerializeObject(jsonString.Result));

                    throw new Exception($"Error occured at Method {System.Reflection.MethodBase.GetCurrentMethod().Name} and Error = Invalid Token!");
                }
                else
                {
                    await SaveErrorLogResponse(00, 0, "createTygaOrerRequest Failed" + Newtonsoft.Json.JsonConvert.SerializeObject(request.ToString()), "result:- " + Newtonsoft.Json.JsonConvert.SerializeObject(jsonString.Result));

                    response = new CreateTygaOrderResponse
                    {
                        Message = "Failed",
                        Data = new CreateOrderResponseData
                        {
                            OrderId = "",
                            PaymentUrl = ""
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                await SaveErrorLogResponse(00, 0, "createTygaOrerRequest Exception", "ex:- " + ex.Message);

                throw new Exception($"Error occured at Method {System.Reflection.MethodBase.GetCurrentMethod().Name} and error ='{ex.Message}'");
            }
            return response;
        }

        public string GenerateHash(object obj, string hashSalt)
        {

            var concatedString = string.Empty;
            var objDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(obj));

            foreach (var pair in objDict)
            {
                if (!string.IsNullOrEmpty(pair.Value))
                {
                    concatedString += ($"{pair.Key}={pair.Value}&");
                }
            }

            concatedString = concatedString.TrimEnd('&');

            var result = GenerateHmacSignature(hashSalt, concatedString);

            return result;
        }
        private string GenerateHmacSignature(string merchantSecret, string payload)
        {
            using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(merchantSecret)))
            {
                var hash = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
                return ToHex(hash, false);
            }
        }

        private string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

        public async Task SaveErrorLogResponse(int associateId, int orderId, string message, string error)
        {
            await _TygaRepository.SaveErrorLogResponse(associateId, orderId, message, error);
        }

        public async Task CreateTygaOrderLogs(int associateId, int orderId, CreateTygaOrderResponse request)
        {
            await _TygaRepository.CreateTygaOrderLogs(associateId, orderId, request);

        }


        public async Task<PaymentStatus> UpdateOrderStatus(TygaPaymentResponse request)
        {
            var tygaOrder = await _TygaRepository.GetTygaOrderbyOrderId(request.OrderId);
            if (string.IsNullOrEmpty(tygaOrder.Status) && string.IsNullOrEmpty(tygaOrder.TransactionNumber))
            {
                var order = await _orderService.GetOrderByOrderNumber(int.Parse(tygaOrder.OrderID));
                var paymentStatus = PaymentStatus.Rejected;
                if (!string.IsNullOrEmpty(request.Status) && (request.Status.ToLower() == "success" || request.Status.ToLower() == "paid"))
                {
                    paymentStatus = PaymentStatus.Accepted;
                    try
                    {
                        await _orderService.FinalizeAcceptedOrder(order);
                    }
                    catch (Exception ex)
                    {
                        await SaveErrorLogResponse(00, 0, "FinalizeAcceptedOrder Exception in Tyga process", "ex:- " + ex.Message);
                    }
                }
                var orderPaymentStatus = await _orderService.FinalizeOrderPaymentStatus(new OrderPaymentStatusUpdate { AuthorizationNumber = request.TxId, PaymentStatus = paymentStatus, TransactionNumber = request.TxId, OrderPaymentId = order.Payments[0].PaymentId, ReferenceNumber = request.TxId, ResponseId = "1", ResponseDescription = "Payment Process from Tyga of Amount:- " + request.Amount });
                try
                {
                    await _TygaRepository.UpdateTygaOrderLogs(request);
                }
                catch(Exception ex) {
                    await SaveErrorLogResponse(00, 0, "UpdateOrderStatus Exception", "ex:- " + ex.Message);
                }
                return orderPaymentStatus.Status;
            }
            return PaymentStatus.NotProvided;
        }

    }
}
