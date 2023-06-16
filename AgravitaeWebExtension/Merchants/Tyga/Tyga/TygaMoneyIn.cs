using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.MoneyIn.Custom.Models;
using DirectScale.Disco.Extension.Services;
using System;
using System.Threading.Tasks;
using AgravitaeExtension.Merchants.Tyga.Interfaces;
using AgravitaeExtension.Merchants.Tyga.Models;

namespace AgravitaeExtension.Merchants.Tyga.Tyga
{
    public class TygaMoneyIn : RedirectMoneyInMerchant
    {
        private readonly IAssociateService _associateService;
        private readonly ITygaService _tygaService;
        private readonly IOrderService _orderService;
        public TygaMoneyIn(ITygaService tygaService, IAssociateService associateService, IOrderService orderService) : base()
        {
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _tygaService = tygaService ?? throw new ArgumentNullException(nameof(tygaService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public async override Task<PaymentRedirectResult> RedirectPayment(int associateId, int orderNumber, Address billingAddress, double amount, string currencyCode, string redirectUrl)
        {
            try
            {
                await _tygaService.SaveErrorLogResponse(associateId, orderNumber, "Tyga ChargePayment Call Start", "RedirectUrl" + redirectUrl);
            }
            catch (Exception ex)
            {
                await _tygaService.SaveErrorLogResponse(00, 00, "Tyga ChargePayment Call Start Exception", "error" + ex.Message);

            }
            if (amount < 1)
            {
                await _tygaService.SaveErrorLogResponse(00, 00, "Amount cannot be less than 1", "error" + $"Amount: {nameof(amount)}");
            }
            


            var associate = await _associateService.GetAssociate(Convert.ToInt32(associateId));

            if (associate == null)
            {
                await _tygaService.SaveErrorLogResponse(00, 00, $"PayorID of { Convert.ToInt32(associateId)} is invalid.", "error");
            }

            var res = new PaymentRedirectResult();
            res.TransactionNumber = Guid.NewGuid().ToString();
            //res.Status = PaymentStatus.Pending;

            try
            {
                var order = await _orderService.GetOrderByOrderNumber(orderNumber);                
                CreateTygaOrderRequest createTygaOrerRequest = 
                    new CreateTygaOrderRequest 
                    { 
                        name = associate.DisplayFirstName, 
                        amount = amount.ToString(), 
                        description = "orderId:- " + orderNumber, 
                        orderNumber = orderNumber.ToString(), 
                        email = associate.EmailAddress, 
                        notifyUrl = "", 
                        returnUrl = redirectUrl 
                    };
                var response = await _tygaService.CreateOrder("/orders", createTygaOrerRequest);
                await _tygaService.CreateTygaOrderLogs(Convert.ToInt32(associateId), orderNumber, response);
                var tygaAmount = amount;
                


                if (!string.IsNullOrEmpty(response.Data.OrderId))
                {
                    //res.Response = response.Data.OrderId;
                    //res.ResponseId = "1";
                    //res.Status = PaymentStatus.Pending;
                    //res.Amount = amount;
                    //res.OrderNumber = 11;
                    //res.PaymentType = "Authorization";
                    //res.Currency = currencyCode.ToUpper();
                    //res.Merchant = MerchantInfo.Id;
                    //res.Redirect = true;
                    //res.RedirectURL = response.Data.PaymentUrl;

                    res.ReferenceNumber = response.Data.OrderId;
                    res.AuthorizationCode = "Authorization";
                    res.RedirectUrl = response.Data.PaymentUrl;

                    return res;
                }
                else
                {
                    //res.Response = response.Message;
                    //res.ResponseId = "2";
                    //res.Status = PaymentStatus.Rejected;

                    res.ReferenceNumber = "2";

                    return res;
                }
            }
            catch (Exception e)
            {
                //res.Response = e.Message;
                //res.ResponseId = "2";
                //res.Status = PaymentStatus.Rejected;

                res.ReferenceNumber = "2";
                await _tygaService.SaveErrorLogResponse(00, orderNumber, "Exception in Tyga ChargePayment ", "Error : " + e.Message);
            }

            return res;
        }

        public async override Task<ExtendedPaymentResult> RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
        {
            //var response = _tygaService.CreditPayment(payerId, orderNumber, currencyCode, paymentAmount, refundAmount, "", transactionNumber, "refund", "");
            return new ExtendedPaymentResult
            {
                Amount = refundAmount,
                AuthorizationCode = authorizationCode,
                Currency = currencyCode.ToUpper(),
                TransactionNumber = transactionNumber,
                ResponseId = orderNumber.ToString(),
                Response = "Refund",
                Status = PaymentStatus.Rejected
            };

        }
        
        //public async override Task<PaymentResponse> RefundPayment(string payerId, int orderNumber, string currencyCode, double paymentAmount, double refundAmount, string referenceNumber, string transactionNumber, string authorizationCode)
        //{
        //    //var response = _tygaService.CreditPayment(payerId, orderNumber, currencyCode, paymentAmount, refundAmount, "", transactionNumber, "refund", "");
        //    return new PaymentResponse
        //    {
        //        Amount = refundAmount,
        //        AuthorizationCode = authorizationCode,
        //        Currency = currencyCode.ToUpper(),
        //        Merchant = MerchantInfo.Id,
        //        OrderNumber = orderNumber,
        //        TransactionNumber = transactionNumber,
        //        ResponseId = "0",
        //        PaymentType = "Refund",
        //        Response = "",
        //        Status = PaymentStatus.Rejected
        //    };

        //}
    }
}